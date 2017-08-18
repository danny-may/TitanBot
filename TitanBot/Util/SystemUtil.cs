namespace System
{
    public static class SystemUtil
    {
        public static bool Between<TElement>(this TElement value, TElement lower, TElement upper, bool lowerInclusive = true, bool upperInclusive = false)
            where TElement : IComparable<TElement>
        {
            var toLower = value.CompareTo(lower);
            var toHigher = value.CompareTo(upper);
            return (toLower > 0 || (toLower >= 0 && lowerInclusive)) &&
                   (toHigher < 0 || (toHigher <= 0 && upperInclusive));
        }
    }
}
