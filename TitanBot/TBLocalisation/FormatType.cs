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
            public static LocalisedString GetName(Formatting.FormatType format)
                => (LocalisedString)($"FORMAT_ID_{(uint)format}_NAME");
            public static LocalisedString GetDescription(Formatting.FormatType format)
                => (LocalisedString)($"FORMAT_ID_{(uint)format}_DESCRIPTION");

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>
                {
                    { GetName(Formatting.FormatType.DEFAULT).Key, "Default" },
                    { GetDescription(Formatting.FormatType.DEFAULT).Key, "Basic formatting, might look a bit rough" }
                }.ToImmutableDictionary();
        }
    }
}
