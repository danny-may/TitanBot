using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.Database;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Scheduler
{
    public class TimerService : ThreadedService
    {
        private TitanbotDatabase _database { get; }
        private TitanbotDependencies _dependencies { get; }

        private IDictionary<EventCallback, ConcurrentBag<Func<TimerContext, Task>>> _callbacks { get; }
            = new ConcurrentDictionary<EventCallback, ConcurrentBag<Func<TimerContext, Task>>>();

        public TimerService(TitanbotDependencies dependencies)
        {
            _database = dependencies.Database;
            _dependencies = dependencies;
            CycleDelay = 1000;
        }

        public override void Initialise()
        {
            base.Initialise();

            RegisterCallbacks();
        }

        protected override async Task Main(DateTime loopTime)
        {
            var timers = await _database.Timers.Get(loopTime);
            var processed = new List<Timer>();
            foreach (var timer in timers)
            {
                var runningFor = (loopTime - timer.From).TotalMilliseconds;
                var msFromCycle = runningFor % (timer.SecondInterval * 1000);
                if (msFromCycle < CycleDelay)
                {
                    var cycleTime = new DateTime(loopTime.Ticks).AddTicks(-(loopTime.Ticks - timer.From.Ticks) % (timer.SecondInterval * TimeSpan.TicksPerSecond));
                    if (!_callbacks.ContainsKey(timer.Callback))
                        continue;
                    var context = new TimerContext(_dependencies, timer, cycleTime);
                    foreach (var callback in _callbacks[timer.Callback])
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        callback.Invoke(context);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                    
                    processed.Add(timer);
                }
            }
            processed = processed.Where(t => t.To.HasValue && t.To.Value < loopTime).ToList();
            if (processed.Count() > 0)
                await _database.Timers.Complete(processed.ToArray());
        }

        public void AddCallback(EventCallback type, Func<TimerContext, Task> callback)
        { 
            if (!_callbacks.ContainsKey(type))
                _callbacks.Add(type, new ConcurrentBag<Func<TimerContext, Task>>());
            _callbacks[type].Add(callback);
        }

        public void RegisterCallbacks()
        {
            AddCallback(EventCallback.TitanLordNow, Callbacks.TitanLordNow);
            AddCallback(EventCallback.TitanLordRound, Callbacks.TitanLordNextRound);
            AddCallback(EventCallback.TitanLordTick, Callbacks.TitanLordTick);
        }

        public async Task AddTimers(IEnumerable<Timer> timers)
        {
            await _database.Timers.Add(timers);
        }
    }

    public enum EventCallback
    {
        TitanLordTick,
        TitanLordNow,
        TitanLordRound,
    }
}
