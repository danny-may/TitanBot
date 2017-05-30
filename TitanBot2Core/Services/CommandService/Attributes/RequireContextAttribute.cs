using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    class RequireContextAttribute : Attribute
    {
        public ContextType RequireContext { get; }

        public RequireContextAttribute(ContextType context)
        {
            RequireContext = context;
        }

        public static ContextType GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<RequireContextAttribute>()?.RequireContext ?? GetFrom(info.ParentInfo);

        public static ContextType GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<RequireContextAttribute>()?.RequireContext ?? ContextType.DM | ContextType.Group | ContextType.Guild;
    }
}
