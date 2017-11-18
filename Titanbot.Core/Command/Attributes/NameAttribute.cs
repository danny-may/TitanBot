using System;
using System.Reflection;
using Titansmasher.Extensions;

namespace Titanbot.Core.Command
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
            => type.GetCustomAttribute<NameAttribute>()?.Value ?? type.Name.RemoveEnd("Command");
        public static bool ExistsOn(Type type)
            => type.GetCustomAttribute<NameAttribute>() != null;

        public static string GetFor(ParameterInfo param)
            => param.GetCustomAttribute<NameAttribute>()?.Value ?? param.Name;
        public static bool ExistsOn(ParameterInfo param)
            => param.GetCustomAttribute<NameAttribute>() != null;
    }
}