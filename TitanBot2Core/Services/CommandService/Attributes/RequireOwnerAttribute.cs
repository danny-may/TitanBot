using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireOwnerAttribute : Attribute
    {
        public static bool GetFrom(CallInfo info)
            => info.Call.GetCustomAttribute<RequireOwnerAttribute>() != null || info.ParentInfo.RequireOwner;

        public static bool GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<RequireOwnerAttribute>() != null;
    }
}
