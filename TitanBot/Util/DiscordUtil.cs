using System;
using System.Linq;
using TitanBot.Logging;

namespace Discord
{
    public static class DiscordUtil
    {
        public static ILoggable ToLoggable(this LogMessage log)
            => new LogEntry(TitanBot.Logging.LogSeverity.Critical, LogType.Client, log.Message, log.Source);

        public static Color ToDiscord(this System.Drawing.Color color)
            => new Color(color.R, color.G, color.B);

        public static bool UserHasPermission(this IChannel channel, IUser user, ChannelPermission permission)
            => (user is IGuildUser guser && channel is IGuildChannel gchannel && guser.GetPermissions(gchannel).Has(permission)) ||
               !(channel is IGuildChannel);

        public static bool HasCharPrefix(this IUserMessage msg, char c, out int argPos)
        {
            var text = msg.Content;
            if (text.Length > 0 && text[0] == c)
            {
                argPos = 1;
                return true;
            }
            argPos = -1;
            return false;
        }
        public static bool HasStringPrefix(this IUserMessage msg, string str, out int argPos, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var text = msg.Content;
            if (text.StartsWith(str, comparisonType))
            {
                argPos = str.Length;
                return true;
            }
            argPos = -1;
            return false;
        }
        public static bool HasMentionPrefix(this IUserMessage msg, IUser user, out int argPos)
        {
            argPos = -1;
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

        public static bool HasAll(this GuildPermissions current, GuildPermissions requireAll)
            => Enum.GetValues(typeof(GuildPermission))
                   .Cast<GuildPermission>()
                   .Where(p => requireAll.Has(p))
                   .All(p => current.Has(p));

        public static bool HasAll(this GuildPermissions current, ulong requireAll)
            => current.HasAll(new GuildPermissions(requireAll));

        public static bool HasAll(this IGuildUser user, GuildPermissions requireAll)
            => user.GuildPermissions.HasAll(requireAll);

        public static bool HasAll(this IGuildUser user, ulong requireAll)
            => user.GuildPermissions.HasAll(new GuildPermissions(requireAll));
    }
}
