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
    public class UsageAttribute : Attribute
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
