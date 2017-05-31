using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static string Tableify(this object[][] data, string cellFormat = "{0} ", string headerFormat = null)
        {
            var builder = new StringBuilder();
            data = data.ForceColumns();
            var maxWidth = data.Rotate().Select(c => c.Max(v => v.ToString().Length)).ToList();
            for (int row = 0; row < data.Length; row++)
            {
                for (int col = 0; col < data[row].Length; col++)
                {
                    if (row == 0 && headerFormat != null)
                        builder.Append(string.Format(headerFormat, data[row][col].ToString().PadRight(maxWidth[col])));
                    else
                        builder.Append(string.Format(cellFormat, data[row][col].ToString().PadRight(maxWidth[col])));
                }
                builder.Append("\n");
            }

            return builder.ToString();
        }

        public static bool EndsWithAny(this string text, params string[] endings)
        {
            return endings.Any(e => text.EndsWith(e));
        }
    }
}
