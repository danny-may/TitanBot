using System;
using System.Reflection;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultPermissionAttribute : Attribute
    {
        #region Statics

        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<DefaultPermissionAttribute>() != null || ExistsOn(method.DeclaringType);

        public static bool ExistsOn(Type type)
            => type.GetCustomAttribute<DefaultPermissionAttribute>() != null;

        public static DefaultPermissionAttribute GetFor(MethodInfo method)
            => method.GetCustomAttribute<DefaultPermissionAttribute>() ?? GetFor(method.DeclaringType);

        public static DefaultPermissionAttribute GetFor(Type type)
            => type.GetCustomAttribute<DefaultPermissionAttribute>() ?? new DefaultPermissionAttribute(0, type.FullName);

        #endregion Statics

        #region Fields

        public ulong DefaultPerm { get; }
        public string PermissionKey { get; }

        #endregion Fields

        #region Constructors

        public DefaultPermissionAttribute(ulong defaultPerm, string permissionKey = null)
        {
            DefaultPerm = defaultPerm;
            PermissionKey = permissionKey;
        }

        #endregion Constructors
    }
}