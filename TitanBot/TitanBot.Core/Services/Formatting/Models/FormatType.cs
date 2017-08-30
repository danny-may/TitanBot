using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Formatting.Models
{
    public struct FormatType : ILocalisable<string>
    {
        public static readonly uint DEFAULT = 0;

        private readonly uint _id;

        internal FormatType(uint type)
        {
            _id = type;
        }

        public bool Equals(FormatType other)
            => _id == other._id;

        public static implicit operator FormatType(uint id)
            => new FormatType(id);

        public static implicit operator uint(FormatType format)
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
            => LocalisableKey.From(GetDescriptionKey(this));

        public ILocalisable<string> GetName()
            => LocalisableKey.From(GetNameKey(this));

        public static string GetNameKey(ulong format)
            => $"FORMAT_ID_{format}_NAME";

        public static string GetDescriptionKey(ulong format)
            => $"FORMAT_ID_{format}_DESCRIPTION";

        public string Localise(ILocalisationCollection localeManager)
            => GetName().Localise(localeManager) ?? "UNKNOWN";

        object ILocalisable.Localise(ILocalisationCollection localeManager)
            => Localise(localeManager);
    }
}