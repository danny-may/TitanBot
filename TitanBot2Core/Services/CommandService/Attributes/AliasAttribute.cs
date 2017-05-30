using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class AliasAttribute : Attribute
    {
        public string[] Aliases { get; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        public static string[] GetFom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<AliasAttribute>()?.Aliases ?? new string[0];
    }
}
