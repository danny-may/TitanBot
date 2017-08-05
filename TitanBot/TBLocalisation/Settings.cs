using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static class Settings
        {
            private const string BASE_PATH = "SETTINGS_";

            public static class Desc
            {
                private const string BASE_PATH = Settings.BASE_PATH + "DESCRIPTION_";

                public const string GUILD_GENERAL = BASE_PATH + nameof(GUILD_GENERAL);
                public const string GLOBAL_GENERAL = BASE_PATH + nameof(GLOBAL_GENERAL);
                public const string USER_GENERAL = BASE_PATH + nameof(USER_GENERAL);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { GUILD_GENERAL, "General guild settings" },
                        { GLOBAL_GENERAL, "General global settings" },
                        { USER_GENERAL, "General user settings" }
                    }.ToImmutableDictionary();
            }

            public static class Notes
            {
                private const string BASE_PATH = Settings.BASE_PATH + "NOTES_";

                public const string GUILD_GENERAL = BASE_PATH + nameof(GUILD_GENERAL);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { GUILD_GENERAL, "For DateTimeFormat, you can use [this link](https://www.codeproject.com/Articles/19677/Formats-for-DateTime-ToString) to help determine what is and is not valid!" }
                    }.ToImmutableDictionary();
            }

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>().Concat(Desc.Defaults)
                                                   .Concat(Notes.Defaults)
                                                   .ToImmutableDictionary();
        }
    }
}
