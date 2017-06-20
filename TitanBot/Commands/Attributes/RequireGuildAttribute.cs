using System;
using System.Reflection;

namespace TitanBot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireGuildAttribute : RequireContextAttribute
    {
        public ulong GuildID { get; }

        public RequireGuildAttribute(ulong guild) : base(ContextType.Guild)
        {
            GuildID = guild;
        }

        new public static ulong? GetFor(Type info)
            => info.GetCustomAttribute<RequireGuildAttribute>()?.GuildID;
        new public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<RequireGuildAttribute>() != null;
    }
}
