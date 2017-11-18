using System;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequireContextAttribute : Attribute
    {
        public ContextType RequireContext { get; }

        public RequireContextAttribute(ContextType context)
        {
            RequireContext = context;
        }

        public static ContextType GetFor(MethodInfo info)
            => info.GetCustomAttribute<RequireContextAttribute>()?.RequireContext ?? GetFor(info.DeclaringType);
        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<RequireContextAttribute>() != null;

        public static ContextType GetFor(Type info)
            => info.GetCustomAttribute<RequireContextAttribute>()?.RequireContext ?? ContextType.DM | ContextType.Group | ContextType.Guild;
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<RequireContextAttribute>() != null;
    }
}