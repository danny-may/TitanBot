using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class NameAttribute : Attribute
    {
        public string Name { get; }

        public NameAttribute(string name)
        {
            Name = name;
        }

        public static string GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<NameAttribute>()?.Name ?? (info.CommandType.Name.EndsWith("Command") ? 
                                                                              info.CommandType.Name.Substring(0, info.CommandType.Name.Length - "Command".Length) : 
                                                                              info.CommandType.Name);

        public static string GetFrom(ArgumentInfo info)
            => GetFrom(info.Parameter);
        public static string GetFrom(ParameterInfo info)
            => info.GetCustomAttribute<NameAttribute>()?.Name ?? info.Name;
    }
}
