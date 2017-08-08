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
                public const string TITLE = BASE_PATH + nameof(TITLE);
                public const string FOOTER = BASE_PATH + nameof(FOOTER);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
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
                        { TITLE, "Current execution statistics" },
                        { FOOTER, "{0} | System Data" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
