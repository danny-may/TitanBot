using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool IsRunning => _mainLoop != null &&
                                !_mainLoop.IsCanceled &&
                                !_mainLoop.IsCompleted &&
                                !_mainLoop.IsFaulted;

        public TitanBotScheduler(IDependencyFactory manager, IDatabase database, ILogger logger)
        {
            _dependencyManager = manager;
            _database = database;
            _logger = logger;
        }

        public ulong Queue<T>(DateTime from) where T : ISchedulerCallback
            => Queue<T>(from, TimeSpan.MaxValue, DateTime.MaxValue);
        public ulong Queue<T>(DateTime from, TimeSpan period) where T : ISchedulerCallback
            => Queue<T>(from, period, DateTime.MaxValue);
        public ulong Queue<T>(DateTime from, DateTime to) where T : ISchedulerCallback
            => Queue<T>(from, TimeSpan.MaxValue, to);
        public ulong Queue<T>(DateTime from, TimeSpan interval, DateTime to) where T : ISchedulerCallback
        {
            var record = new TitanBotSchedulerRecord
            {
                Callback = typeof(T),
                EndTime = to,
                StartTime = from,
                Interval = interval
            };
            _database.Insert(record);
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
                var completed = actives.Where(r => r.EndTime < pollTime);
                var ongoing = actives.Where(r => r.EndTime > pollTime);
                await Cancel(completed);
                foreach (var record in completed)
                {
                    if (!TryGetHandler(record.Callback, out ISchedulerCallback callback))
                        continue;
                    Task.Run(() => callback.Handle(record.Id, record.StartTime, record.Interval, record.EndTime, record.EndTime))
                        .DontWait();
                }
                foreach (var record in ongoing)
                {
                    var intervalDelta = (pollTime - record.StartTime).TotalMilliseconds % record.Interval.TotalMilliseconds;
                    if (intervalDelta > PollingPeriod || !TryGetHandler(record.Callback, out ISchedulerCallback callback))
                        continue;
                    Task.Run(() => callback.Handle(record.Id, record.StartTime, record.Interval, record.EndTime, pollTime.AddMilliseconds(-intervalDelta)))
                        .DontWait();
                }
            }
        }

        private bool TryGetHandler(Type type, out ISchedulerCallback callback)
        {
            callback = null;
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

        public Task Cancel(ulong id)
            => Cancel(new List<ulong> { id });

        public async Task Cancel(IEnumerable<ulong> ids)
            => await Cancel(await _database.FindById<TitanBotSchedulerRecord>(ids));

        private Task Cancel(IEnumerable<TitanBotSchedulerRecord> records)
            => _database.Upsert(records.ForEach(r => { r.Complete = true; return r; }));

        public async Task<int> ActiveCount()
            => (await FindActives(DateTime.Now)).Count();
    }
}
