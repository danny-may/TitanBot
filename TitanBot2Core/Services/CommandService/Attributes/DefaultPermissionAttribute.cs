using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultPermissionAttribute : Attribute
    {
        public ulong DefaultPerm { get; }
        private string _permissionKey { get; }

        public DefaultPermissionAttribute(ulong defaultPerm, string permissionKey = null)
        {
            DefaultPerm = defaultPerm;
            _permissionKey = permissionKey;
        }

        public static string GetKey(CallInfo info)
            => info.Call.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? info.ParentInfo.PermissionKey;

        public static ulong GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<DefaultPermissionAttribute>()?.DefaultPerm ?? info.ParentInfo.DefaultPermissions;

        public static string GetKey(CommandInfo info)
            => info.CommandType.GetCustomAttribute<DefaultPermissionAttribute>()?._permissionKey ?? info.Name;

        public static ulong GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<DefaultPermissionAttribute>()?.DefaultPerm ?? 0;
    }
}
