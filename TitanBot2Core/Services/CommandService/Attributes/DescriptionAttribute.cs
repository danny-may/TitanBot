using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public static string GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }
}
