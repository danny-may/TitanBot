using System;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CallAttribute : Attribute
    {
        #region Statics

        public static string GetFor(MethodInfo method)
            => method?.GetCustomAttribute<CallAttribute>()?.CallName;
        public static bool ExistsOn(MethodInfo method)
            => method?.GetCustomAttribute<CallAttribute>() != null;

        #endregion Statics

        #region Fields

        public string CallName { get; }

        #endregion Fields

        #region Constuctors

        public CallAttribute(string callName = null)
            => CallName = callName;

        #endregion Constuctors
    }
}