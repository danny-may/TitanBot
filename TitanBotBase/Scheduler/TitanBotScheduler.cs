using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;
using TitanBotBase.Util;

namespace TitanBotBase.Scheduler
{
    public class TitanBotScheduler : IScheduler
    {
        private readonly IDatabase _database;
        private readonly IDependencyFactory _dependencyManager;
        private readonly ILogger _logger;
        private readonly Dictionary<Type, object> _cachedHandlers = new Dictionary<Type, object>();
        private Task _mainLoop;
        private bool _shouldStop;
        public int PollingPeriod = 1000;
        private static ulong PrevId { get; set; }

        public bool IsRunning => _mainLoop != null &&
                                !_mainLoop.IsCanceled &&
                                !_mainLoop.IsCompleted &&
                                !_mainLoop.IsFaulted;

        public TitanBotScheduler(IDependencyFactory manager, IDatabase database, ILogger logger)
        {
            _dependencyManager = manager;
            _database = database;
            _logger = logger;
            PrevId = _database.Find<TitanBotSchedulerRecord>(r => true).Result.Select(r => r.Id).OrderByDescending(r => r).FirstOrDefault();
        }



        public ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = default(TimeSpan?), DateTime? to = default(DateTime?), string data = null) where T : ISchedulerCallback
        {
            var record = new TitanBotSchedulerRecord
            {
                Id = ++PrevId,
                Callback = JsonConvert.SerializeObject(typeof(T)),
                UserId = userId,
                GuildId = guildID,
                EndTime = to ?? DateTime.MaxValue,
                StartTime = from,
                Interval = period ?? TimeSpan.MaxValue,
                Data = data
            };
            _database.Insert(record).Wait();
            return record.Id;
        }

        public Task StartAsync()
        {
            if (IsRunning)
                throw new InvalidOperationException("Scheduler is already running");
            _shouldStop = false;
            _mainLoop = Task.Run((Action)MainLoop);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            if (_shouldStop || !IsRunning)
                throw new InvalidOperationException("Scheduler is already stopped/stopping");
            _shouldStop = true;
            return _mainLoop;
        }

        private async void MainLoop()
        {
            var pollTime = DateTime.MinValue;
            while (!_shouldStop)
            {
                var msSincePrev = (int)(DateTime.Now - pollTime).TotalMilliseconds;
                if (pollTime != DateTime.MinValue && msSincePrev > PollingPeriod)
                    _logger.Log(LogSeverity.Critical, LogType.Scheduler, $"Scheduler is overburdened. Lost {msSincePrev - PollingPeriod}ms", "MainLoop");
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
            if (!_cachedHandlers.ContainsKey(type))
            {
                if (!_dependencyManager.TryConstruct(type, out object constructed))
                    return false;
                _cachedHandlers.Add(type, constructed);
            }
            callback = (ISchedulerCallback)_cachedHandlers[type];
            return true;
        }

        private Task<IEnumerable<TitanBotSchedulerRecord>> FindActives(DateTime pollTime)
            => _database.Find((TitanBotSchedulerRecord r) => !r.Complete && r.StartTime < pollTime);

        public ISchedulerRecord[] Complete(IEnumerable<ulong> ids, bool wasCancelled = true)
            => Complete(_database.FindById<TitanBotSchedulerRecord>(ids).Result, wasCancelled);

        private ISchedulerRecord[] Complete(IEnumerable<TitanBotSchedulerRecord> records, bool wasCancelled = true)
        {
            foreach (var record in records)
            {
                if (TryGetHandler(record.Callback, out ISchedulerCallback callback))
                    Task.Run(() => callback.Complete(record, wasCancelled)).DontWait();
                record.Complete = true;
                record.EndTime = DateTime.Now;
            }
            _database.Upsert(records).Wait();
            return records.ToArray();
        }

        public int ActiveCount()
            => FindActives(DateTime.Now).Result.Count();

        public ISchedulerRecord[] Complete<T>(ulong? guildId, ulong? userId, bool wasCancelled = true) where T : ISchedulerCallback
        {
            var callback = typeof(T);
            var type = JsonConvert.SerializeObject(typeof(T));
            IEnumerable<TitanBotSchedulerRecord> records;
            if (userId != null)
                records = _database.Find<TitanBotSchedulerRecord>(r => !r.Complete && r.GuildId == guildId && r.Callback == type && r.UserId == userId).Result;
            else 
                records = _database.Find<TitanBotSchedulerRecord>(r => !r.Complete && r.GuildId == guildId && r.Callback == type).Result;

            return Complete(records, wasCancelled);
        }

        public ISchedulerRecord Complete(ulong id, bool wasCancelled = true)
            => Complete(new ulong[] { id }, wasCancelled).First();

        public ISchedulerRecord GetMostRecent<T>(ulong guildId) where T : ISchedulerCallback
        {
            var type = JsonConvert.SerializeObject(typeof(T));
            return _database.Find<TitanBotSchedulerRecord>(r => r.Callback == type && r.GuildId == guildId).Result.OrderByDescending(r => r.EndTime).FirstOrDefault();
        }
    }
}
