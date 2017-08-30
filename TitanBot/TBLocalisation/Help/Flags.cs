using System.Collections.Generic;
using System.Collections.Immutable;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static partial class Help
        {
            public static class Flags
            {
                private const string BASE_PATH = Help.BASE_PATH + "FLAG_";

                public const string EXCEPTION_F = BASE_PATH + nameof(EXCEPTION_F);
                public const string PREFERENCES_G = BASE_PATH + nameof(PREFERENCES_G);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { EXCEPTION_F, "Returns the full exception in a file" },
                        { PREFERENCES_G, "Specifies what group you are interested in" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
