using System;
using System.Linq;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GroupAttribute : Attribute
    {
        public string GroupName { get; }

        public GroupAttribute(string name)
        {
            GroupName = name;
        }

        public static string GetFor(Type info)
            => info.GetCustomAttribute<GroupAttribute>()?.GroupName ?? info.Namespace.Split('.').Last();
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<GroupAttribute>() != null;
    }
}