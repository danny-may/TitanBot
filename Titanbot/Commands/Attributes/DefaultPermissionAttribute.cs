using System;
using System.Reflection;
using Titanbot.Commands.Models;

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

        public static PermissionModel GetFor(MethodInfo method)
        {
            var methodAttr = method.GetCustomAttribute<DefaultPermissionAttribute>();
            var typeAttr = method.DeclaringType.GetCustomAttribute<DefaultPermissionAttribute>();

            var key = (typeAttr?.PermissionKey ?? method.DeclaringType.Name) + "." + (methodAttr?.PermissionKey ?? method.Name);
            var perm = methodAttr?.DefaultPerm ?? typeAttr?.DefaultPerm ?? 0;

            return new PermissionModel(key, perm);
        }

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