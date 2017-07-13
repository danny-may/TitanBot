using System;

namespace TitanBot.Models
{
    public abstract class Range<T> where T : IComparable<T>
    {
        protected delegate bool TryParseDelegate(string text, out T value);

        public T From { get; set; }
        public T To { get; set; }

        protected static bool TryParse<R>(TryParseDelegate tryParse, Func<R> builder, string text, out R value)
            where R : Range<T>
        {
            value = null;
            if (text == null)
                return false;

            var parts = text.Split('-');

            if (parts.Length == 0)
                return false;

            if (parts.Length == 1)
                parts = new string[] { parts[0], parts[0] };

            if (parts.Length != 2)
                return false;

            if (!tryParse(parts[0], out var from) || !tryParse(parts[1], out var to))
                return false;

            value = builder();

            if (from.CompareTo(to) < 0)
                (from, to) = (to, from);

            value.From = from;
            value.To = to;
            return true;
        }
    }

    public class IntRange : Range<int>
    {
        public static bool TryParse(string text, out IntRange value)
            => TryParse(int.TryParse, () => new IntRange(), text, out value);
    }
    public class UIntRange : Range<uint>
    {
        public static bool TryParse(string text, out UIntRange value)
            => TryParse(uint.TryParse, () => new UIntRange(), text, out value);
    }
    public class DoubleRange : Range<double>
    {
        public static bool TryParse(string text, out DoubleRange value)
            => TryParse(double.TryParse, () => new DoubleRange(), text, out value);
    }
    public class LongRange : Range<long>
    {
        public static bool TryParse(string text, out LongRange value)
            => TryParse(long.TryParse, () => new LongRange(), text, out value);
    }
    public class ULongRange : Range<ulong>
    {
        public static bool TryParse(string text, out ULongRange value)
            => TryParse(ulong.TryParse, () => new ULongRange(), text, out value);
    }
    public class ShortRange : Range<short>
    {
        public static bool TryParse(string text, out ShortRange value)
            => TryParse(short.TryParse, () => new ShortRange(), text, out value);
    }
    public class UShortRange : Range<ushort>
    {
        public static bool TryParse(string text, out UShortRange value)
            => TryParse(ushort.TryParse, () => new UShortRange(), text, out value);
    }
    public class DecimalRange : Range<decimal>
    {
        public static bool TryParse(string text, out DecimalRange value)
            => TryParse(decimal.TryParse, () => new DecimalRange(), text, out value);
    }
    public class FloatRange : Range<float>
    {
        public static bool TryParse(string text, out FloatRange value)
            => TryParse(float.TryParse, () => new FloatRange(), text, out value);
    }
    public class CharRange : Range<char>
    {
        public static bool TryParse(string text, out CharRange value)
            => TryParse(char.TryParse, () => new CharRange(), text, out value);
    }
    public class DateTimeRange : Range<DateTime>
    {
        public static bool TryParse(string text, out DateTimeRange value)
            => TryParse(DateTime.TryParse, () => new DateTimeRange(), text, out value);
    }
}
