using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CallAttribute : Attribute
    {
        private string Value { get; }

        public CallAttribute(string subCommands = null)
        {
            Value = subCommands;
        }

        public static string GetFor(MethodInfo method)
            => method.GetCustomAttribute<CallAttribute>()?.Value;
        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<CallAttribute>() != null;
    }
}
