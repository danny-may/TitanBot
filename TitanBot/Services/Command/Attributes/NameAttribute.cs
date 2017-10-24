using System;
using System.Reflection;

namespace TitanBot.Services.Command
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class NameAttribute : Attribute
    {
        private string Value { get; }

        public NameAttribute(string name = null)
        {
            Value = name;
        }

        public static string GetFor(Type type)
            => type.GetCustomAttribute<NameAttribute>()?.Value ?? (type.Name.EndsWith("command", StringComparison.InvariantCultureIgnoreCase) ? type.Name.Substring(0, type.Name.Length - "command".Length) : type.Name);
        public static bool ExistsOn(Type type)
            => type.GetCustomAttribute<NameAttribute>() != null;

        public static string GetFor(ParameterInfo param)
            => param.GetCustomAttribute<NameAttribute>()?.Value ?? param.Name;
        public static bool ExistsOn(ParameterInfo param)
            => param.GetCustomAttribute<NameAttribute>() != null;
    }
}
