using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class DefaultPermissionAttribute : Attribute
    {
        private ulong _defaultPerm { get; }
        private string _permissionKey { get; }

        public DefaultPermissionAttribute(ulong defaultPerm, string permissionKey = null)
        {
            _defaultPerm = defaultPerm;
            _permissionKey = permissionKey;
        }

        public static string GetKey(CallInfo info)
        {
            if (info.Call.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey != null)
                return info.ParentInfo.PermissionKey + ("." + info.Call.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? "").TrimEnd('.');
            else
                return info.ParentInfo.PermissionKey + ("." + string.Join(".", info.Subcalls)).TrimEnd('.');
        }

        public static ulong GetPerm(CallInfo info)
            => info.Call.GetCustomAttribute<DefaultPermissionAttribute>()?._defaultPerm ?? info.ParentInfo.DefaultPermissions;

        public static string GetKey(CommandInfo info)
            => info.CommandType.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? info.Name;

        public static ulong GetPerm(CommandInfo info)
            => info.CommandType.GetCustomAttribute<DefaultPermissionAttribute>()?._defaultPerm ?? 0;
    }
}
