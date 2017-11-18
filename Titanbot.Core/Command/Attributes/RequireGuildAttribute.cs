using System;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireGuildAttribute : RequireContextAttribute
    {
        public ulong[] GuildIDs { get; }

        public RequireGuildAttribute(params ulong[] guild) : base(ContextType.Guild)
        {
            GuildIDs = guild;
        }

        new public static ulong[] GetFor(Type info)
            => info.GetCustomAttribute<RequireGuildAttribute>()?.GuildIDs ?? new ulong[0];
        new public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<RequireGuildAttribute>() != null;
    }
}