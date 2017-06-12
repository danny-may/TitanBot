using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UsageAttribute : Attribute
    {
        public string Usage { get; }

        public UsageAttribute(string usage)
        {
            Usage = usage;
        }

        public static string GetFor(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>()?.Usage;
        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>() != null;
    }
}
