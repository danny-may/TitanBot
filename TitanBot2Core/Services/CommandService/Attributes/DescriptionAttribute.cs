using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
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
