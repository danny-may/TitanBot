using System;
using System.Linq;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class GroupAttribute : Attribute
    {
        public string GroupName { get; }

        public GroupAttribute(string name)
        {
            GroupName = name;
        }

        public static string GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<GroupAttribute>()?.GroupName ?? info.CommandType.Namespace.Split('.').Last();
    }
}
