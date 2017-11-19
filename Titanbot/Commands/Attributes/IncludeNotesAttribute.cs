using System;
using System.Reflection;
using Titansmasher.Extensions;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IncludeNotesAttribute : DisplayableBaseAttribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(Type info)
            => info.GetCustomAttribute<IncludeNotesAttribute>()?.GetNote(info);

        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<IncludeNotesAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Note { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public IncludeNotesAttribute(string note, DisplayType displayType = DisplayType.Key)
        {
            Note = note.NullIfWhitespace();
            DisplayType = displayType;
        }

        public IncludeNotesAttribute()
            : this(null)
        {
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetNote(Type info)
            => DisplayType.BuildFor(Note ?? GetTranslationKey(info, "note"));

        #endregion Methods
    }
}