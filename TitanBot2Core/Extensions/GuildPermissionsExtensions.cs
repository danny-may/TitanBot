using Discord;
using System;
using System.Linq;

namespace TitanBot2.Extensions
{
    public static class GuildPermissionsExtensions
    {
        public static bool HasAll(this GuildPermissions current, GuildPermissions requireAll)
        {
            return Enum.GetValues(typeof(GuildPermission))
                       .Cast<GuildPermission>()
                       .Where(p => requireAll.Has(p))
                       .All(p => current.Has(p));
        }

        public static bool HasAll(this GuildPermissions current, ulong requireAll)
        {
            return current.HasAll(new GuildPermissions(requireAll));
        }
    }
}
