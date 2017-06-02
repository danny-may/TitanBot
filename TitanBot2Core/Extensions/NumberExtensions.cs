using System;

namespace TitanBot2.Extensions
{
    public static class NumberExtensions
    {
        public static int Clamp(this int value, int lower, int upper)
        {
            EnsureOrder(ref lower, ref upper);
            if (value < lower)
                value = lower;
            if (value > upper)
                value = upper;

            return value;
        }
        public static void Clamp(ref int value, int lower, int upper)
            => value = value.Clamp(lower, upper);

        public static double Clamp(this double value, double lower, double upper)
        {
            EnsureOrder(ref lower, ref upper);
            if (value < lower)
                value = lower;
            if (value > upper)
                value = upper;

            return value;
        }
        public static void Clamp(ref double value, double lower, double upper)
            => value = value.Clamp(lower, upper);

        public static void EnsureOrder(ref int lower, ref int higher)
        {
            var temp = Math.Min(lower, higher);
            higher = Math.Max(lower, higher);
            lower = temp;
        }

        public static void EnsureOrder(ref double lower, ref double higher)
        {
            var temp = Math.Min(lower, higher);
            higher = Math.Max(lower, higher);
            lower = temp;
        }
    }
}
