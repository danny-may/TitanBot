using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
