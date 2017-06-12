using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public static string GetFor(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>()?.Description;
        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>() != null;
    }
}
