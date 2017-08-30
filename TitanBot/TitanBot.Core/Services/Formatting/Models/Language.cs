using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Formatting.Models
{
    public struct Language
    {
        public static readonly Language DEFAULT = "English";

        public static readonly HashSet<Language> KnownLanguages = new HashSet<Language>();

        private readonly string _id;
        private readonly string _idUpper;

        private Language(string id)
        {
            _id = id;
            _idUpper = id.ToUpperInvariant();
            KnownLanguages.Add(this);
        }

        public override bool Equals(object obj)
            => obj is Language other && other._idUpper == _idUpper;

        public override int GetHashCode()
            => _idUpper.GetHashCode();

        public static implicit operator Language(string id)
            => new Language(id);

        public static implicit operator string(Language lang)
            => lang._id;

        public static bool operator ==(Language l1, Language l2)
            => l1._idUpper == l2._idUpper;

        public static bool operator !=(Language l1, Language l2)
             => l1._idUpper != l2._idUpper;
    }
}