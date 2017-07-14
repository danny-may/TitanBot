using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Formatting
{
    public struct Locale
    {
        public static Locale DEFAULT => "DEFAULT";

        private string _locale { get; }
        private static List<Locale> _knownLocales { get; } = new List<Locale>();

        public static IReadOnlyList<Locale> KnownLocales => _knownLocales.AsReadOnly();

        internal Locale(string locale)
        {
            _locale = locale.ToUpper();
            _knownLocales.Add(this);
        }

        public override string ToString()
            => _locale;

        public bool Equals(Locale other)
            => _locale == other._locale;

        public static implicit operator Locale(string locale)
            => new Locale(locale);

        public static bool operator ==(Locale locale1, Locale locale2)
            => locale1._locale == locale2._locale;

        public static bool operator !=(Locale locale1, Locale locale2)
            => locale1._locale != locale2._locale;

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
