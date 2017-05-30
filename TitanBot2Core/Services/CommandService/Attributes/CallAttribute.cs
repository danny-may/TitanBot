using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CallAttribute : Attribute
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
