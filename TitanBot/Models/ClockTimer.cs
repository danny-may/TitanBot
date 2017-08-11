using System;
using System.Threading.Tasks;

namespace TitanBot.Models
{
    public delegate void ClockTimerElapsedEventHandler(object sender, ClockTimerElapsedEventArgs e);

    public class ClockTimer
    {
        /// <summary>
        /// The offset from 00:00:00 for the timer to start at. Max value is Interval
        /// </summary>
        public TimeSpan Offset { get => _offset; set { _offset = value; UpdateBaseline(); } }
        /// <summary>
        /// The time between calls
        /// </summary>
        public TimeSpan Interval { get => _interval; set { _interval = value; UpdateBaseline(); } }
        public bool Enabled { get => _enabled; set { _enabled = value; RunClock(); } }

        public DateTime BaseTime { get; private set; }

        public event ClockTimerElapsedEventHandler Elapsed;
        public event ClockTimerElapsedEventHandler ElapsedAsync;

        private TimeSpan _offset;
        private TimeSpan _interval;
        private bool _enabled;

        private void UpdateBaseline()
        {
            var actualOffset = new TimeSpan(_offset.Ticks % _interval.Ticks);
            var start = DateTime.MinValue.Add(actualOffset);
            var delta = (DateTime.Now - start).Ticks;
            var shift = delta - (delta % _interval.Ticks);
            BaseTime = start.AddTicks(shift);
        }

        private DateTime NextInterval
            => DateTime.Now.Add(TimeToNextInterval);
        private TimeSpan TimeToNextInterval
            => Interval - new TimeSpan((DateTime.Now - BaseTime).Ticks % Interval.Ticks);

        public async void RunClock()
        {
            while (Enabled)
            {
                var nextInterval = NextInterval;
                await Task.Delay(TimeToNextInterval);
                var args = new ClockTimerElapsedEventArgs(nextInterval);
                OnCycle(args);
                await Task.Delay(1);
            }
        }

        private void OnCycle(ClockTimerElapsedEventArgs args)
        {
            Elapsed?.Invoke(this, args);
            if (ElapsedAsync != null)
                Task.Run(() => ElapsedAsync(this, args));
        }
    }

    public class ClockTimerElapsedEventArgs
    {
        /// <summary>
        /// The actual time when the event was fired
        /// </summary>
        public DateTime SignalTime { get; }
        /// <summary>
        /// In the case that the clock is running slow, this will be set to the difference between ElapsedTime and when the event should have occured
        /// </summary>
        public TimeSpan Slowdown { get; }

        public ClockTimerElapsedEventArgs(DateTime signalTime)
        {
            SignalTime = signalTime;
            Slowdown = DateTime.Now - SignalTime;
        }
    }
}
