using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string text, char delim, bool respectQuotes)
        {
            if (!respectQuotes)
                return Regex.Matches(text, $@"[^{delim}]+")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();

            var result = new List<string>();
            var escaped = false;
            var quote = false;
            var current = "";
            foreach (var c in text)
            {
                if (escaped)
                {
                    current += c;
                    escaped = false;
                }
                else
                {
                    switch (c)
                    {
                        case '\\':
                            escaped = true;
                            break;
                        case '"':
                            quote = !quote;
                            break;
                        default:
                            if (c == delim && !quote)
                            {
                                result.Add(current);
                                current = "";
                            }
                            else
                                current += c;
                            break;
                    }
                }
            }
            result.Add(current);
            return result.ToArray();
        }
    }
}
