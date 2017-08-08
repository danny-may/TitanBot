using TitanBot.Formatting.Interfaces;

namespace TitanBot.Formatting
{
    public struct FormatType : ILocalisable<string>
    {
        public const uint DEFAULT = 0;

        private uint _id { get; }

        internal FormatType(uint type)
        {
            _id = type;
        }

        public bool Equals(FormatType other)
            => _id == other._id;

        public static implicit operator FormatType(uint id)
            => new FormatType(id);

        public  static implicit operator uint(FormatType format)
            => format._id;

        public static bool operator ==(FormatType format1, FormatType format2)
            => format1._id == format2._id;

        public static bool operator !=(FormatType format1, FormatType format2)
            => format1._id != format2._id;

        public override bool Equals(object obj)
            => obj is FormatType f ? _id.Equals(f._id) : obj is uint u ? _id.Equals(u) : false;

        public override int GetHashCode()
            => _id.GetHashCode();

        public ILocalisable<string> GetDescription()
            => TBLocalisation.FormatType.GetDescription(this);

        public ILocalisable<string> GetName()
            => TBLocalisation.FormatType.GetName(this);

        public string Localise(ITextResourceCollection textResource)
            => GetName().Localise(textResource) ?? "UNKNOWN";

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
    }
}
