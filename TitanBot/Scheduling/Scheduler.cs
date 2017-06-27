using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Dependencies;
using TitanBot.Logging;
using TitanBot.Storage;
using TitanBot.Util;

namespace TitanBot.Scheduling
{
    public class Scheduler : IScheduler
    {
        private readonly IDatabase Database;
        private readonly IDependencyFactory DependencyManager;
        private readonly ILogger Logger;
        private readonly Dictionary<Type, object> CachedHandlers = new Dictionary<Type, object>();
        private Task LoopTask;
        private bool ShouldStop;
        public int PollingPeriod = 1000;

        public bool IsRunning => LoopTask != null &&
                                !LoopTask.IsCanceled &&
                                !LoopTask.IsFaulted;

        public Scheduler(IDependencyFactory manager, IDatabase database, ILogger logger)
        {
            DependencyManager = manager;
            Database = database;
            Logger = logger;
        }



        public ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = default(TimeSpan?), DateTime? to = default(DateTime?), string data = null) where T : ISchedulerCallback
        {
            var record = new SchedulerRecord
            {
                Callback = JsonConvert.SerializeObject(typeof(T)),
                UserId = userId,
                GuildId = guildID,
                EndTime = to ?? DateTime.MaxValue,
                StartTime = from,
                Interval = period ?? TimeSpan.MaxValue,
                Data = data
            };
            Database.Insert(record).Wait();
            return record.Id;
        }

        public Task StartAsync()
        {
            if (IsRunning)
                throw new InvalidOperationException("Scheduler is already running");
            ShouldStop = false;
            LoopTask = Task.Run((Action)MainLoop);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (ShouldStop || !IsRunning)
                throw new InvalidOperationException("Scheduler is already stopped/stopping");
            ShouldStop = true;
            return LoopTask;
        }

        private async void MainLoop()
        {
            var pollTime = DateTime.MinValue;
            while (!ShouldStop)
            {
                var msSincePrev = (int)(DateTime.Now - pollTime).TotalMilliseconds;
                if (pollTime != DateTime.MinValue && msSincePrev > PollingPeriod)
                    Logger.Log(LogSeverity.Critical, LogType.Scheduler, $"Scheduler is overburdened. Lost {msSincePrev - PollingPeriod}ms", "MainLoop");
                await Task.Delay((PollingPeriod - msSincePrev).Clamp(0, int.MaxValue));
                pollTime = DateTime.Now;
                var actives = await FindActives(pollTime);
                var completed = actives.Where(r => r.EndTime < pollTime).ToList();
                var ongoing = actives.Where(r => r.EndTime > pollTime).ToList();
                Complete(completed, false);
                foreach (var record in ongoing)
                {
                    var intervalDelta = (pollTime - record.StartTime).Ticks % record.Interval.Ticks;
                    if (intervalDelta/10000 > PollingPeriod || !TryGetHandler(record.Callback, out ISchedulerCallback callback))
                        continue;
                    Task.Run(() => callback.Handle(record, pollTime.AddTicks(-intervalDelta)))
                        .DontWait();
                }
            }
        }

        private bool TryGetHandler(string serialisedType, out ISchedulerCallback callback)
        {
            callback = null;
            var type = JsonConvert.DeserializeObject<Type>(serialisedType);
            if (!CachedHandlers.ContainsKey(type))
            {
                if (!DependencyManager.TryConstruct(type, out object constructed))
                    return false;
                CachedHandlers.Add(type, constructed);
            }
            callback = (ISchedulerCallback)CachedHandlers[type];
            return true;
        }

        private Task<IEnumerable<SchedulerRecord>> FindActives(DateTime pollTime)
            => Database.Find((SchedulerRecord r) => !r.Complete && r.StartTime < pollTime);

        public ISchedulerRecord[] Complete(IEnumerable<ulong> ids, bool wasCancelled = true)
            => Complete(Database.FindById<SchedulerRecord>(ids).Result, wasCancelled);

        private ISchedulerRecord[] Complete(IEnumerable<SchedulerRecord> records, bool wasCancelled = true)
        {
            foreach (var record in records)
            {
                if (TryGetHandler(record.Callback, out ISchedulerCallback callback))
                    Task.Run(() => callback.Complete(record, wasCancelled)).DontWait();
                record.Complete = true;
                record.EndTime = DateTime.Now;
            }
            Database.Upsert(records).Wait();
            return records.ToArray();
        }

        public int ActiveCount()
            => FindActives(DateTime.Now).Result.Count();

        public ISchedulerRecord[] Complete<T>(ulong? guildId, ulong? userId, bool wasCancelled = true) where T : ISchedulerCallback
        {
            var callback = typeof(T);
            var type = JsonConvert.SerializeObject(typeof(T));
            IEnumerable<SchedulerRecord> records;
            if (userId != null)
                records = Database.Find<SchedulerRecord>(r => !r.Complete && r.GuildId == guildId && r.Callback == type && r.UserId == userId).Result;
            else 
                records = Database.Find<SchedulerRecord>(r => !r.Complete && r.GuildId == guildId && r.Callback == type).Result;

            return Complete(records, wasCancelled);
        }

        public ISchedulerRecord Complete(ulong id, bool wasCancelled = true)
            => Complete(new ulong[] { id }, wasCancelled).First();

        public ISchedulerRecord GetMostRecent<T>(ulong guildId) where T : ISchedulerCallback
        {
            var type = JsonConvert.SerializeObject(typeof(T));
            return Database.Find((SchedulerRecord r) => r.Callback == type && r.GuildId == guildId).Result.OrderByDescending(r => r.EndTime).FirstOrDefault();
        }
    }
}
