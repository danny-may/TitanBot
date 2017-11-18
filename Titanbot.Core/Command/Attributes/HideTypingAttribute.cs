using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HideTypingAttribute : Attribute
    {
        public HideTypingAttribute()
        {
        }

        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<CallAttribute>() != null;
    }
}