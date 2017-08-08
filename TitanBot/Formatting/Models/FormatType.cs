using TitanBot.Formatting.Interfaces;
using static TitanBot.TBLocalisation.FormatType;

namespace TitanBot.Formatting
{
    public struct FormatType : ILocalisable<string>
    {
        public const uint DEFAULT = 0;

        private uint _id { get; }
        private LocalisedString Name { get; }

        internal FormatType(uint type)
        {
            _id = type;
            Name = null;
            Name = FromFormat(this);
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
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();

        public string Localise(ITextResourceCollection textResource)
            => Name?.Localise(textResource) ?? "UNKNOWN";

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
    }
}
