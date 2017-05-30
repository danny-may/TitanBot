using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class RequireGuildAttribute : RequireContextAttribute
    {
        public ulong GuildID { get; }

        public RequireGuildAttribute(ulong guild) : base(ContextType.Guild)
        {
            GuildID = guild;
        }

        public static ulong? GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<RequireGuildAttribute>()?.GuildID;
    }
}
