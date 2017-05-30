using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class CallAttribute : Attribute
    {
        public string[] SubCommands { get; }

        public CallAttribute(params string[] subCommands)
        {
            SubCommands = subCommands;
        }

        public static string[] GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<CallAttribute>()?.SubCommands ?? new string[0];
    }
}
