namespace TitanBot.Core.Services.Formatting.Models
{
    public struct FormatType : IDisplayable<string>
    {
        #region Statics

        public static readonly uint DEFAULT = 0;

        public static string GetDescriptionKey(ulong format)
            => $"FORMAT_ID_{format}_DESCRIPTION";

        public static string GetNameKey(ulong format)
            => $"FORMAT_ID_{format}_NAME";

        #endregion Statics

        #region Fields

        private readonly uint _id;

        #endregion Fields

        #region Constructors

        internal FormatType(uint type)
        {
            _id = type;
        }

        #endregion Constructors

        #region Methods

        public bool Equals(FormatType other)
            => _id == other._id;

        public IDisplayable<string> GetDescription()
            => TransKey.From(GetDescriptionKey(this));

        public IDisplayable<string> GetName()
            => TransKey.From(GetNameKey(this));

        #endregion Methods

        #region Overrides

        public override bool Equals(object obj)
            => obj is FormatType f ? _id.Equals(f._id) : obj is uint u ? _id.Equals(u) : false;

        public override int GetHashCode()
            => _id.GetHashCode();

        #endregion Overrides

        #region Operators

        public static implicit operator FormatType(uint id)
            => new FormatType(id);

        public static implicit operator uint(FormatType format)
            => format._id;

        public static bool operator !=(FormatType format1, FormatType format2)
            => format1._id != format2._id;

        public static bool operator ==(FormatType format1, FormatType format2)
            => format1._id == format2._id;

        #endregion Operators

        #region IDisplayable

        public string Display(ITranslationSet translations, IValueFormatter formatter)
            => GetName().Display(translations, formatter) ?? "UNKNOWN";

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        #endregion IDisplayable
    }
}