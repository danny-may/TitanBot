namespace Titansmasher.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveEnd(this string input, string toRemove)
            => input.EndsWith(toRemove) ? input.Substring(0, input.Length - toRemove.Length) : input;

        public static string RemoveStart(this string input, string toRemove)
            => input.StartsWith(toRemove) ? input.Substring(toRemove.Length - 1) : input;

        public static string NullIfWhitespace(this string input)
            => string.IsNullOrWhiteSpace(input) ? null : input;
    }
}