using System;
using System.Linq;
using System.Reflection;

namespace TitanBot.Services.Command
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class LiteralAttribute : Attribute
    {
        private object[] Values { get; }

        public LiteralAttribute(object value, params object[] values)
            => Values = new object[] { value }.Concat(values).ToArray();

        public static bool ExistsOn(ParameterInfo param)
            => param.GetCustomAttribute<LiteralAttribute>() != null;
        public static object[] GetFor(ParameterInfo param)
            => param.GetCustomAttribute<LiteralAttribute>()?.Values ?? new[] { NameAttribute.GetFor(param) };
    }
}
