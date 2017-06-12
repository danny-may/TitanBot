using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotesAttribute : Attribute
    {
        public string Notes { get; }

        public NotesAttribute(string notes)
        {
            Notes = notes;
        }

        public static string GetFor(Type info)
            => info.GetCustomAttribute<NotesAttribute>()?.Notes;
        public static bool EistsOn(Type info)
            => info.GetCustomAttribute<NotesAttribute>() != null;
    }
}
