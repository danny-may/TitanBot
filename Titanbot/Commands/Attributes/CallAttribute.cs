using System;
using System.Reflection;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CallAttribute : Attribute
    {
        #region Statics

        public static string[] GetFor(MethodInfo method)
            => method?.GetCustomAttribute<CallAttribute>()?.ArgumentMask ?? new string[0];
        public static bool ExistsOn(MethodInfo method)
            => method?.GetCustomAttribute<CallAttribute>() != null;

        #endregion Statics

        #region Fields

        public string[] ArgumentMask { get; }

        #endregion Fields

        #region Constuctors

        public CallAttribute(params string[] mask)
            => ArgumentMask = mask;

        #endregion Constuctors
    }
}