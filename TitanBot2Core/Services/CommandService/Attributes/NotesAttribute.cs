using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class NotesAttribute : Attribute
    {
        public string Notes { get; }

        public NotesAttribute(string notes)
        {
            Notes = notes;
        }

        public static string GetFrom(CommandInfo info)
            => info.CommandType.GetCustomAttribute<NotesAttribute>()?.Notes;
    }
}
