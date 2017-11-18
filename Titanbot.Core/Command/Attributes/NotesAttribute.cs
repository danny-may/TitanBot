using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotesAttribute : Attribute
    {
        #region Statics

        public static string /*ILocalisable<string>*/ GetFor(Type info)
            => info.GetCustomAttribute<NotesAttribute>()?.GetNote();
        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<NotesAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Note { get; }
        //public LocalisationType LocalisationType { get; }

        #endregion Fields

        #region Constructors

        public NotesAttribute(string note)//, LocalisationType localisationType = LocalisationType.Key)
        {
            Note = note;
            //LocalisationType = LocalisationType;
        }

        #endregion Constructors

        #region Methods

        //private ILocalisable<string> GetNote()
        //    => LocalisationType.BuildNew(Note);

        private string GetNote()
            => Note;

        #endregion Methods
    }
}