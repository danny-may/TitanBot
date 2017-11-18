using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HiddenAttribute : Attribute
    {
        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<HiddenAttribute>() != null || ExistsOn(info.DeclaringType);

        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<HiddenAttribute>() != null;
    }
}