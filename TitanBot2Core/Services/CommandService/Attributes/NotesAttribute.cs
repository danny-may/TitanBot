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
    public class NotesAttribute : Attribute
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
