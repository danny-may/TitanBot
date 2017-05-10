using Discord;
using System;

namespace TitanBot2.Extensions
{
    public static class GuildPermissionExtension
    {
        public static ulong Value(this GuildPermission perm)
        {
            return (ulong)Math.Pow(2, ((uint)perm));
        }
    }
}
