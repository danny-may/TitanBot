using System.Collections.Generic;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titansmasher.Services.Displaying
{
    public struct Language : IDisplayable<string>
    {
        #region Statics

        public static readonly Language DEFAULT = 0;

        public static readonly HashSet<Language> KnownLanguages = new HashSet<Language>();

        public static string GetNameKey(int id)
            => $"LANUGAGE_{id}_NAME";

        #endregion Statics

        #region Fields

        private readonly int _id;

        #endregion Fields

        #region Constructors

        private Language(int id)
        {
            _id = id;
            KnownLanguages.Add(this);
        }

        #endregion Constructors

        #region Methods

        public string GetNameKey()
            => GetNameKey(this);

        #endregion Methods

        #region Overrides

        public override bool Equals(object obj)
            => obj is Language other && other._id == _id;

        public override int GetHashCode()
            => _id.GetHashCode();

        #endregion Overrides

        #region Operators

        public static implicit operator Language(int id)
            => new Language(id);

        public static implicit operator int(Language lang)
            => lang._id;

        public static bool operator ==(Language l1, Language l2)
            => l1._id == l2._id;

        public static bool operator !=(Language l1, Language l2)
             => l1._id != l2._id;

        #endregion Operators

        #region IDisplayable

        object IDisplayable.Display(IDisplayService display, DisplayOptions options)
            => Display(display, options);

        public string Display(IDisplayService display, DisplayOptions options = default)
            => new Translation(GetNameKey(this)).Display(display, options);

        #endregion IDisplayable
    }
}