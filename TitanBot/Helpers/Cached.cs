using System;
using System.Threading.Tasks;

namespace TitanBot.Helpers
{
    public delegate void CacheUpdateEventHandler<T>(object sender, CacheUpdateEventArgs<T> args);

    public static class Cached
    {
        public static Cached<T> FromValue<T>(T value) => Cached<T>.FromValue(value);

        public static Cached<T> FromSource<T>(Func<T> source, TimeSpan? validFor = null) => Cached<T>.FromSource(source, validFor);
        public static Cached<T> FromSource<T>(Func<ValueTask<T>> source, TimeSpan? validFor = null) => Cached<T>.FromSource(source, validFor);
    }

    public class Cached<T>
    {
        private T _value;
        private DateTime _lastUpdate = DateTime.MinValue;
        private Func<ValueTask<T>> _source { get; }

        public event CacheUpdateEventHandler<T> OnUpdate;

        public T Value => GetValue();
        public TimeSpan? InvalidationPeriod { get; }
        public bool IsFresh => _lastUpdate != DateTime.MinValue &&
                               (InvalidationPeriod == null || (DateTime.Now - _lastUpdate < InvalidationPeriod));

        public static Cached<T> FromValue(T value) => new Cached<T>(() => new ValueTask<T>(value), TimeSpan.MaxValue);

        public static Cached<T> FromSource(Func<T> source, TimeSpan? validFor = null) => FromSource(() => new ValueTask<T>(source()), validFor);
        public static Cached<T> FromSource(Func<ValueTask<T>> source, TimeSpan? validFor = null) => new Cached<T>(source, validFor);

        private Cached(Func<ValueTask<T>> source, TimeSpan? validFor)
        {
            _source = source;
            InvalidationPeriod = validFor;
        }

        public void Invalidate()
            => _lastUpdate = DateTime.MinValue;

        public T GetValue()
        {
            if (!IsFresh)
                Update().Wait();
            return _value;
        }

        private async Task Update()
        {
            var old = _value;
            var start = DateTime.Now;
            _value = await _source();
            _lastUpdate = DateTime.Now;

            if (!old?.Equals(_value) ?? _value == null)
                OnUpdate?.Invoke(this, new CacheUpdateEventArgs<T>(old, _value, start, DateTime.Now));
        }
    }

    public class CacheUpdateEventArgs<T>
    {
        public T OldValue { get; }
        public T NewValue { get; }
        public DateTime UpdateStart { get; }
        public DateTime UpdateFinish { get; }
        public TimeSpan UpdateDuration => UpdateFinish - UpdateStart;

        internal CacheUpdateEventArgs(T old, T @new, DateTime start, DateTime finish)
        {
            OldValue = old;
            NewValue = @new;
            UpdateStart = start;
            UpdateFinish = finish;
        }
    }
}
