﻿using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Dependencies;
using TitanBot.Logging;
using TitanBot.Storage;

namespace TitanBot.Scheduling
{
    public class Scheduler : IScheduler
    {
        private readonly IDatabase Database;
        private readonly IDependencyFactory DependencyManager;
        private readonly ILogger Logger;
        private readonly DiscordSocketClient Client;
        private Task LoopTask;
        private bool ShouldStop;
        public int PollingPeriod = 1000;

        public bool IsRunning => LoopTask != null &&
                                !LoopTask.IsCanceled &&
                                !LoopTask.IsFaulted;

        public Scheduler(DiscordSocketClient client, IDependencyFactory manager, IDatabase database, ILogger logger)
        {
            Client = client;
            DependencyManager = manager;
            Database = database;
            Logger = logger;
        }



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
                await LogErrors(async () =>
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
                        if (intervalDelta / 10000 > PollingPeriod || !TryGetHandler(record.Callback, out ISchedulerCallback callback))
                            continue;
                        Task.Run(() => LogErrors(() => callback.Handle(new SchedulerContext(record, Client, DependencyManager), pollTime.AddTicks(-intervalDelta))))
                            .DontWait();
                    }
                });
            }
        }

        private async void LogErrors(Action action)
            => await LogErrors(() => { action(); return Task.CompletedTask; });

        private async Task LogErrors(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                await Database.Upsert(new Error
                {
                    Channel = null,
                    Content = ex.ToString(),
                    Description = ex.Message,
                    Message = null,
                    Type = ex.GetType().Name,
                    User = null
                });
            }
        }

        private bool TryGetHandler(string serialisedType, out ISchedulerCallback callback)
        {
            var type = JsonConvert.DeserializeObject<Type>(serialisedType);
            callback = null;
            if (!DependencyManager.TryConstruct(type, out object constructed))
                return false;
            callback = (ISchedulerCallback)constructed;
            return true;
        }

        private ValueTask<IEnumerable<SchedulerRecord>> FindActives(DateTime pollTime)
            => Database.Find((SchedulerRecord r) => !r.Complete && r.StartTime < pollTime);

        public ISchedulerRecord[] Complete(IEnumerable<ulong> ids, bool wasCancelled = true)
            => Complete(Database.FindById<SchedulerRecord>(ids).Result, wasCancelled);

        private ISchedulerRecord[] Complete(IEnumerable<SchedulerRecord> records, bool wasCancelled = true)
        {
            foreach (var record in records)
            {
                if (TryGetHandler(record.Callback, out ISchedulerCallback callback))
                    Task.Run(() => callback.Complete(new SchedulerContext(record, Client, DependencyManager), wasCancelled)).DontWait();
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
