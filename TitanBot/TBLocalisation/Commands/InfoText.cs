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
        public static partial class Commands
        {
            public static class InfoText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "INFO_";

                public const string LANGUAGE_EMBED_DESCRIPTION = BASE_PATH + nameof(LANGUAGE_EMBED_DESCRIPTION);
                public const string FIELD_GUILDS = BASE_PATH + nameof(FIELD_GUILDS);
                public const string FIELD_CHANNELS = BASE_PATH + nameof(FIELD_CHANNELS);
                public const string FIELD_USERS = BASE_PATH + nameof(FIELD_USERS);
                public const string FIELD_COMMANDS = BASE_PATH + nameof(FIELD_COMMANDS);
                public const string FIELD_CALLS = BASE_PATH + nameof(FIELD_CALLS);
                public const string FIELD_COMMANDS_USED = BASE_PATH + nameof(FIELD_COMMANDS_USED);
                public const string FIELD_DATABASE_QUERIES = BASE_PATH + nameof(FIELD_DATABASE_QUERIES);
                public const string FIELD_FACTORY_BUILT = BASE_PATH + nameof(FIELD_FACTORY_BUILT);
                public const string FIELD_RAM = BASE_PATH + nameof(FIELD_RAM);
                public const string FIELD_CPU = BASE_PATH + nameof(FIELD_CPU);
                public const string FIELD_TIMERS = BASE_PATH + nameof(FIELD_TIMERS);
                public const string FIELD_UPTIME = BASE_PATH + nameof(FIELD_UPTIME);
                public const string LANGUAGE_COVERAGE = BASE_PATH + nameof(LANGUAGE_COVERAGE);
                public const string TECHNICAL_TITLE = BASE_PATH + nameof(TECHNICAL_TITLE);
                public const string TECHNICAL_FOOTER = BASE_PATH + nameof(TECHNICAL_FOOTER);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { LANGUAGE_EMBED_DESCRIPTION, "Here are all the supported languages by me!" },
                        { FIELD_GUILDS, "Guilds" },
                        { FIELD_CHANNELS, "Channels" },
                        { FIELD_USERS, "Users" },
                        { FIELD_COMMANDS, "Loaded commands" },
                        { FIELD_CALLS, "Loaded calls" },
                        { FIELD_COMMANDS_USED, "Commands used this session" },
                        { FIELD_DATABASE_QUERIES, "Database queries" },
                        { FIELD_FACTORY_BUILT, "Factory Objects produced" },
                        { FIELD_RAM, "RAM" },
                        { FIELD_CPU, "CPU" },
                        { FIELD_TIMERS, "Active Timers" },
                        { FIELD_UPTIME, "Uptime" },
                        { LANGUAGE_COVERAGE, "{0}% Coverage" },
                        { TECHNICAL_TITLE, "Current execution statistics" },
                        { TECHNICAL_FOOTER, "{0} | System Data" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
