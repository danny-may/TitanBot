using System;

namespace TitanBot.Formatting
{
    public struct Locale
    {
        public const string DEFAULT = "Default";
        
        private string _id { get; }
        private string _idUpper { get; }

        private Locale(string id)
        {
            _id = id.ToLower().ToTitleCase();
            _idUpper = id.ToUpper();
        }

        public override string ToString()
            => _id;

        public bool Equals(Locale other)
            => _idUpper == other._idUpper;

        public static implicit operator Locale(string id)
            => new Locale(id);

        public static implicit operator string(Locale locale)
            => locale.ToString();

        public static bool operator ==(Locale locale1, Locale locale2)
            => locale1._idUpper == locale2._idUpper;

        public static bool operator !=(Locale locale1, Locale locale2)
            => locale1._idUpper != locale2._idUpper;

        public override bool Equals(object obj)
            => (obj is Locale l && this == l) || (obj is string s && this == s);

        public override int GetHashCode()
            => _idUpper.GetHashCode();
    }
}
