using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultPermissionAttribute : Attribute
    {
        private ulong _defaultPerm { get; }
        private string _permissionKey { get; }

        public DefaultPermissionAttribute(ulong defaultPerm, string permissionKey = null)
        {
            _defaultPerm = defaultPerm;
            _permissionKey = permissionKey;
        }

        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey != null || ExistsOn(info.DeclaringType);
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey != null;

        public static string GetKeyFor(MethodInfo info)
        {
            if (info.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey != null)
                return GetKeyFor(info.DeclaringType) + ("." + info.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? "").TrimEnd('.');
            else
                return GetKeyFor(info.DeclaringType) + ("." + CallAttribute.GetFor(info)).TrimEnd('.');
        }

        public static ulong GetPermFor(MethodInfo info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?._defaultPerm ?? GetPermFor(info.DeclaringType);

        public static string GetKeyFor(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? NameAttribute.GetFor(info);

        public static ulong GetPermFor(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?._defaultPerm ?? 0;
    }
}
