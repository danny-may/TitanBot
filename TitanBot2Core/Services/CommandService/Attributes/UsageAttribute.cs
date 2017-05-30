using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class UsageAttribute : Attribute
    {
        public string Usage { get; }

        public UsageAttribute(string usage)
        {
            Usage = usage;
        }

        public static string GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<UsageAttribute>()?.Usage;
    }
}
