using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Dependencies;
using TitanBot.Helpers;
using TitanBot.Logging;
using TitanBot.Storage;

namespace TitanBot.Scheduling
{
    public class Scheduler : IScheduler
    {
        private readonly IDatabase Database;
        private readonly IDependencyFactory DependencyFactory;
        private readonly ILogger Logger;
        private readonly DiscordSocketClient Client;
        private ClockTimer MainLoop { get; }
        private Cached<List<SchedulerRecord>> CachedRecords { get; }
        private CachedDictionary<string, Type> CachedTypes { get; }
        private CachedDictionary<string, ISchedulerCallback> CachedHandlers { get; }

        public double PollingPeriod { get => MainLoop.Interval.TotalMilliseconds; set => MainLoop.Interval = new TimeSpan(0, 0, 0, 0, (int)value); }
        public bool Enabled { get => MainLoop.Enabled; set => MainLoop.Enabled = value; }
        private DateTime StartTime { get; set; }

        public Scheduler(DiscordSocketClient client, IDependencyFactory factory, IDatabase database, ILogger logger)
        {
            Client = client;
            DependencyFactory = factory;
            Database = database;
            Logger = logger;
            CachedRecords = Cached.FromSource(GetRecords, new TimeSpan(0, 30, 0));
            CachedTypes = CachedDictionary.FromSource((Func<string, Type>)DeserialiseType);
            CachedHandlers = CachedDictionary.FromSource((Func<string, ISchedulerCallback>)GetCallback);
            MainLoop = new ClockTimer();
            PollingPeriod = 1000;

            MainLoop.Elapsed += Cycle;
        }

        private ISchedulerCallback GetCallback(string callbackType)
        {
            if (CachedTypes[callbackType] != null && DependencyFactory.TryGetOrConstruct(CachedTypes[callbackType], out var callback))
                return (ISchedulerCallback)callback;
            return null;
        }

        private Type DeserialiseType(string callbackType)
        {
            try
            {
                return JsonConvert.DeserializeObject<Type>(callbackType);
            }
            catch { }
            return null;

        }

        private async ValueTask<List<SchedulerRecord>> GetRecords()
            => (await Database.Find<SchedulerRecord>(r => !r.IsComplete)).ToList();

        private void Cycle(ClockTimer sender, ClockTimerElapsedEventArgs e)
        {
            if (e.Slowdown > sender.Interval)
                Logger.Log(LogSeverity.Critical, LogType.Scheduler, $"Scheduler is overburdened. Lost {e.Slowdown.TotalMilliseconds - sender.Interval.TotalMilliseconds}ms", "MainLoop");
            Logger.Log(LogSeverity.Debug, LogType.Scheduler, $"{e.SignalTime} Slowdown: {e.Slowdown}", "SchedulerCycle");
            try
            {
                var records = GetActive(e.SignalTime);
                var completed = new List<SchedulerRecord>();
                Parallel.ForEach(records, record =>
                {
                    if (record.EndTime < e.SignalTime)
                        completed.Add(record);
                    else
                    {
                        var absDelta = (e.SignalTime - record.StartTime).Ticks;
                        var intervalDelta = (e.SignalTime - record.StartTime).Ticks % record.Interval.Ticks;
                        if (absDelta < record.Interval.Ticks || intervalDelta / 10000 > PollingPeriod || CachedHandlers[record.Callback] == null)
                            return;
                        try
                        {
                            CachedHandlers[record.Callback].Handle(new SchedulerContext(record, Client, e, DependencyFactory), e.SignalTime.AddTicks(-intervalDelta));
                        }
                        catch (Exception ex)
                        {
                            Log(ex);
                        }
                    }
                });
                if (completed.Count > 0)
                    Complete(completed.ToArray(), e, false);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private async void Log(Exception ex)
        {
            await Database.Insert(new Error
            {
                Content = ex.ToString(),
                Description = ex.Message,
                Type = ex.GetType().Name
            });
        }

        private SchedulerRecord[] FindById(ulong[] ids)
        {
            var cached = CachedRecords.Value.Where(r => ids.Contains(r.Id));
            if (cached.Count() != 0)
                return cached.ToArray();
            return Database.FindById<SchedulerRecord>(ids).Result.ToArray();
        }
        private SchedulerRecord[] Find(Expression<Func<SchedulerRecord, bool>> predicate)
        {
            var cached = CachedRecords.Value.Where(predicate.Compile());
            if (cached.Count() != 0)
                return cached.ToArray();
            return Database.Find(predicate).Result.ToArray();
        }

        private List<SchedulerRecord> GetActive(DateTime atTime)
            => CachedRecords.Value.Where(r => r.StartTime < atTime && (r.CompleteTime ?? DateTime.MaxValue) > atTime).ToList();

        public ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = default(TimeSpan?), DateTime? to = default(DateTime?), ulong? message = null, ulong? channel = null, string data = null) where T : ISchedulerCallback
        {
            var record = new SchedulerRecord
            {
                Callback = JsonConvert.SerializeObject(typeof(T)),
                UserId = userId,
                GuildId = guildID,
                EndTime = to ?? DateTime.MaxValue,
                StartTime = from,
                Interval = period ?? TimeSpan.MaxValue,
                MessageId = message,
                ChannelId = channel,
                Data = data
            };
            Database.Insert(record).Wait();
            CachedRecords.Value.Add(record);
            return record.Id;
        }

        public ISchedulerRecord[] Cancel<T>(ulong? guildId, ulong? userId)
            where T : ISchedulerCallback
        {
            var type = JsonConvert.SerializeObject(typeof(T));
            Expression<Func<SchedulerRecord, bool>> predicate;
            if (userId != null)
                predicate = r => !r.IsComplete && r.GuildId == guildId && r.Callback == type && r.UserId == userId;
            else
                predicate = r => !r.IsComplete && r.GuildId == guildId && r.Callback == type;

            return Complete(Find(predicate), null, true);
        }
        public ISchedulerRecord Cancel(ulong id)
            => Cancel(new ulong[] { id })[0];
        public ISchedulerRecord[] Cancel(ulong[] ids)
            => Complete(FindById(ids), null, true);
        private ISchedulerRecord[] Complete(SchedulerRecord[] records, ClockTimerElapsedEventArgs e, bool wasCancelled)
        {
            var completeTime = DateTime.Now;
            records = records.Where(r => r != null).ToArray();
            foreach (var record in records)
            {
                record.CompleteTime = completeTime;
                if (CachedHandlers[record.Callback] != null)
                    try
                    {
                        CachedHandlers[record.Callback].Complete(new SchedulerContext(record, Client, e, DependencyFactory), wasCancelled);
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
            }
            Database.Upsert<SchedulerRecord>(records).Wait();
            return records.ToArray();
        }

        public async ValueTask<int> PruneBefore(DateTime date)
        {
            var res = await Database.Delete<SchedulerRecord>(r => r.IsComplete && r.CompleteTime < date);
            CachedRecords.Invalidate();
            return res;
        }


        public int ActiveCount()
            => GetActive(DateTime.Now).Count;

        public ISchedulerRecord GetMostRecent<T>(ulong? guildId, ulong? userid) where T : ISchedulerCallback
        {
            var type = JsonConvert.SerializeObject(typeof(T));
            var initial = Find(r => r.Callback == type && r.GuildId == guildId);
            if (userid != null)
                initial = initial.Where(r => r.UserId == userid).ToArray();
            return initial.OrderByDescending(r => r.EndTime)
                           .ThenByDescending(r => r.StartTime)
                           .FirstOrDefault();
        }

        public void PreRegister<T>(T handler) where T : ISchedulerCallback
        {
            var key = JsonConvert.SerializeObject(typeof(T));
            CachedHandlers[key] = handler;
            CachedTypes[key] = typeof(T);

        }

        public void Cancel(Expression<Func<ISchedulerRecord, bool>> predicate)
        {
            Complete(Find(Expression.Lambda<Func<SchedulerRecord, bool>>(predicate.Body, Expression.Parameter(typeof(SchedulerRecord)))).ToArray(), null, true);
        }
    }
}
