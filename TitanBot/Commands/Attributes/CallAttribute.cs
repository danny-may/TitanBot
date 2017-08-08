using System;
using System.Reflection;

namespace TitanBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HideTypingAttribute : Attribute
    {
        public HideTypingAttribute() { }
        
        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<CallAttribute>() != null;
    }
}
