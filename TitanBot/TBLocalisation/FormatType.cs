using System.Collections.Generic;
using System.Collections.Immutable;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static class FormatType
        {
            public static LocalisedString FromFormat(Formatting.FormatType format)
                => (LocalisedString)("FORMAT_ID_" + (uint)format);

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>
                {
                    { FromFormat(Formatting.FormatType.DEFAULT).Key, "Default" }
                }.ToImmutableDictionary();
        }
    }
}
