using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class RequireOwnerAttribute : Attribute
    {
        public static bool GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<RequireOwnerAttribute>() != null || info.ParentInfo.RequireOwner;

        public static bool GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<RequireOwnerAttribute>() != null;
    }
}
