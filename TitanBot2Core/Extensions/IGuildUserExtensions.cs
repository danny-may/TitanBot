using Discord;

namespace TitanBot2.Extensions
{
    public static class IGuildUserExtensions
    {
        public static bool HasAll(this IGuildUser user, GuildPermissions requireAll)
        {
            return user.GuildPermissions.HasAll(requireAll);
        }

        public static bool HasAll(this IGuildUser user, ulong requireAll)
        {
            return user.GuildPermissions.HasAll(new GuildPermissions(requireAll));
        }
    }
}
