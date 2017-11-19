using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Titansmasher.Services.Display.Interfaces;

namespace Titansmasher.Services.Display
{
    public sealed class Language : IDisplayable<string>
    {
        #region Statics

        private readonly static HashSet<Language> _known = new HashSet<Language>();

        public static Language Default { get; } = nameof(Default);

        public static Language Get(string s)
            => _known.FirstOrDefault(l => string.Equals(l, s, StringComparison.InvariantCultureIgnoreCase)) ?? new Language(s);

        #endregion Statics

        #region Fields

        private string _id;
        private Translation _display;

        #endregion Fields

        #region Constructors

        private Language(string id)
        {
            _id = !string.IsNullOrWhiteSpace(id)
                        ? id.ToLower()
                        : throw new ArgumentException("Argument must not be an empty string", nameof(id));
            _id = Regex.Replace(_id, @"[^a-zA-Z]", "");
            _display = new Translation($"language.{id}.name");
            _known.Add(this);
        }

        #endregion Constructors

        #region Overrides

        public override bool Equals(object obj)
            => (obj is Language l && string.Equals(l._id, _id, StringComparison.InvariantCultureIgnoreCase)) ||
               (obj is string s && string.Equals(s, _id, StringComparison.InvariantCultureIgnoreCase));

        public override string ToString()
            => _id;

        public override int GetHashCode()
            => _id.GetHashCode();

        #endregion Overrides

        #region Operators

        #region Implicit

        public static implicit operator string(Language l)
            => l._id;

        public static implicit operator Language(string s)
            => Get(s);

        #endregion Implicit

        #endregion Operators

        #region IDisplayable

        public string Display(IDisplayService service, DisplayOptions options = default)
            => _display.Display(service, options);

        object IDisplayable.Display(IDisplayService service, DisplayOptions options)
            => Display(service, options);

        #endregion IDisplayable
    }
}