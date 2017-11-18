using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NoReplyAttribute : Attribute
    {
        #region Statics

        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<NoReplyAttribute>() != null || ExistsOn(method.DeclaringType);

        public static bool ExistsOn(Type type)
            => type.GetCustomAttribute<NoReplyAttribute>() != null;

        #endregion Statics
    }
}