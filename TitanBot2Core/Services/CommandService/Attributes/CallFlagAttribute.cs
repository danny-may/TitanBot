using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CallFlagAttribute : Attribute
    {
        public FlagInfo Flag { get; }

        public CallFlagAttribute(Type type, string shortKey)
            : this(type, shortKey, null, null) { }
        public CallFlagAttribute(Type type, string shortKey, string description)
            : this(type, shortKey, null, description) { }
        public CallFlagAttribute(Type type, string shortKey, string longKey, string description)
            => Flag = new FlagInfo(type, shortKey, longKey, description);

        public CallFlagAttribute(string shortKey)
            : this(shortKey, null, null) { }
        public CallFlagAttribute(string shortKey, string description)
            : this(shortKey, null, description) { }
        public CallFlagAttribute(string shortKey, string longKey, string description)
            => Flag = new FlagInfo(typeof(bool), shortKey, longKey, description);


        public static FlagInfo[] GetFrom(CallInfo info)
            => info.Call.GetCustomAttributes<CallFlagAttribute>().Select(a => a.Flag).ToArray();
    }
}
