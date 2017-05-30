using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TitanBot2.Extensions
{
    public static class IUserMessageExtensions
    {
        public static bool HasMentionPrefix(this IUserMessage message, IUser user)
        {
            int x = 0;
            return message.HasMentionPrefix(user, ref x);
        }

        public static bool HasStringPrefix(this IUserMessage message, string text)
        {
            int x = 0;
            return message.HasStringPrefix(text, ref x);
        }

        public static bool HasMentionPrefix(this IUserMessage msg, IUser user, ref int argPos)
        {
            var text = msg.Content;
            if (text.Length <= 3 || text[0] != '<' || text[1] != '@') return false;

            int endPos = text.IndexOf('>');
            if (endPos == -1) return false;
            if (text.Length < endPos + 2 || text[endPos + 1] != ' ') return false; //Must end in "> "

            ulong userId;
            if (!MentionUtils.TryParseUser(text.Substring(0, endPos + 1), out userId)) return false;
            if (userId == user.Id)
            {
                argPos = endPos + 2;
                return true;
            }
            return false;
        }
        public static bool HasStringPrefix(this IUserMessage msg, string str, ref int argPos, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var text = msg.Content;
            if (text.StartsWith(str, comparisonType))
            {
                argPos = str.Length;
                return true;
            }
            return false;
        }
    }
}
