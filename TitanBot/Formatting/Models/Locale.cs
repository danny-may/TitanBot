namespace TitanBot.Formatting
{
    public struct Locale
    {
        public static Locale DEFAULT { get; } = new Locale("Default");
        
        private string _id { get; set; }

        private Locale(string id)
            => _id = id;

        public override string ToString()
            => _id;

        public bool Equals(Locale other)
            => _id == other._id;

        public static implicit operator Locale(string id)
            => new Locale(id);

        public static implicit operator string(Locale locale)
            => locale._id;

        public static bool operator ==(Locale locale1, Locale locale2)
            => locale1._id.ToUpper() == locale2._id.ToUpper();

        public static bool operator !=(Locale locale1, Locale locale2)
            => locale1._id.ToUpper() != locale2._id.ToUpper();

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
