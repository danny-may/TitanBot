using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TitanBot2.Extensions
{
    public static class IUserMessageExtensions
    {
        public static string[] GetArguments(this IUserMessage message, int argPos)
        {
            var args = new List<string>();
            var current = new StringBuilder();
            var isEscaped = false;
            var isQuoted = false;
            var isArray = false;
            foreach (var c in message.Content.Substring(argPos))
            {
                if (isEscaped)
                {
                    current.Append(c);
                    isEscaped = false;
                }
                else
                {
                    switch (c)
                    {
                        case '\\':
                            isEscaped = true;
                            break;
                        case ',':
                            isArray = true;
                            current.Append(c);
                            break;
                        case '"':
                            isQuoted = !isQuoted;
                            break;
                        case ' ':
                            if (isQuoted || isArray)
                                current.Append(c);
                            else
                            {
                                args.Add(current.ToString());
                                current.Clear();
                            }
                            break;
                        default:
                            current.Append(c);
                            isArray = false;
                            break;
                    }
                }
            }

            args.Add(current.ToString());

            return args.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }
    }
}
