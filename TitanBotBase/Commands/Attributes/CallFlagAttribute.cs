using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class CallFlagAttribute : Attribute
    {
        public char ShortKey { get; }
        public string LongKey { get; }
        public string Description { get; }

        public CallFlagAttribute(char shortKey, string description)
        {
            ShortKey = shortKey;
            Description = description;
        }
        public CallFlagAttribute(char shortKey, string longKey, string description)
        {
            ShortKey = shortKey;
            LongKey = longKey;
            Description = description;
        }

        private FlagDefinition BuildFrom(ParameterInfo info)
            => new FlagDefinition(this, info);

        public static FlagDefinition GetFor(ParameterInfo info)
            => info.GetCustomAttribute<CallFlagAttribute>()?.BuildFrom(info);
        public static bool ExistsOn(ParameterInfo info)
            => info.GetCustomAttribute<CallFlagAttribute>() != null;
    }
}
