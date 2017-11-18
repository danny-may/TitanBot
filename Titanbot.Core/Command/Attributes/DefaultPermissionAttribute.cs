using System;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultPermissionAttribute : Attribute
    {
        private ulong DefaultPerm { get; }
        private string PermissionKey { get; }

        public DefaultPermissionAttribute(ulong defaultPerm, string permissionKey = null)
        {
            DefaultPerm = defaultPerm;
            PermissionKey = permissionKey;
        }

        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?.PermissionKey != null || ExistsOn(info.DeclaringType);
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?.PermissionKey != null;

        public static string GetKeyFor(MethodInfo info)
        {
            if (info.GetCustomAttribute<DefaultPermissionAttribute>()?.PermissionKey != null)
                return GetKeyFor(info.DeclaringType) + ("." + info.GetCustomAttribute<DefaultPermissionAttribute>()?.PermissionKey ?? "").TrimEnd('.');
            else
                return GetKeyFor(info.DeclaringType) + ("." + CallAttribute.GetFor(info)).TrimEnd('.');
        }

        public static ulong GetPermFor(MethodInfo info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?.DefaultPerm ?? GetPermFor(info.DeclaringType);

        public static string GetKeyFor(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?.PermissionKey ?? NameAttribute.GetFor(info);

        public static ulong GetPermFor(Type info)
            => info.GetCustomAttribute<DefaultPermissionAttribute>()?.DefaultPerm ?? 0;
    }
}