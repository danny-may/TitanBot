using System;
using System.Reflection;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotesAttribute : Attribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(Type info)
            => info.GetCustomAttribute<NotesAttribute>()?.GetNote();
        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<NotesAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Note { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public NotesAttribute(string note, DisplayType displayType = DisplayType.Key)
        {
            Note = note;
            DisplayType = displayType;
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetNote()
            => DisplayType.BuildFor(Note);

        #endregion Methods
    }
}