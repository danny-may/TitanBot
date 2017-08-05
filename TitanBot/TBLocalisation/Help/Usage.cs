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
        public static partial class Help
        {
            public static class Usage
            {
                private const string BASE_PATH = Help.BASE_PATH + "USAGE_";

                public const string PING = BASE_PATH + nameof(PING);
                public const string EDITCOMMAND_SETROLE = BASE_PATH + nameof(EDITCOMMAND_SETROLE);
                public const string EDITCOMMAND_SETPERM = BASE_PATH + nameof(EDITCOMMAND_SETPERM);
                public const string EDITCOMMAND_RESET = BASE_PATH + nameof(EDITCOMMAND_RESET);
                public const string EDITCOMMAND_BLACKLIST = BASE_PATH + nameof(EDITCOMMAND_BLACKLIST);
                public const string SETTINGS_DEFAULT = BASE_PATH + nameof(SETTINGS_DEFAULT);
                public const string SETTINGS_TOGGLE = BASE_PATH + nameof(SETTINGS_TOGGLE);
                public const string SETTINGS_SET = BASE_PATH + nameof(SETTINGS_SET);
                public const string SETTINGSRESET_THISGUILD = BASE_PATH + nameof(SETTINGSRESET_THISGUILD);
                public const string SETTINGSRESET_GIVENGUILD = BASE_PATH + nameof(SETTINGSRESET_GIVENGUILD);
                public const string ABOUT = BASE_PATH + nameof(ABOUT);
                public const string DONATE = BASE_PATH + nameof(DONATE);
                public const string HELP = BASE_PATH + nameof(HELP);
                public const string HELP_TUTORIAL = BASE_PATH + nameof(HELP_TUTORIAL);
                public const string INFO_LANGUAGE = BASE_PATH + nameof(INFO_LANGUAGE);
                public const string INFO_TECHNICAL = BASE_PATH + nameof(INFO_TECHNICAL);
                public const string INVITE = BASE_PATH + nameof(INVITE);
                public const string PREFIX_SHOW = BASE_PATH + nameof(PREFIX_SHOW);
                public const string PREFIX_SET = BASE_PATH + nameof(PREFIX_SET);
                public const string DBPURGE = BASE_PATH + nameof(DBPURGE);
                public const string EXEC = BASE_PATH + nameof(EXEC);
                public const string GLOBALSETTINGS_DEFAULT = BASE_PATH + nameof(GLOBALSETTINGS_DEFAULT);
                public const string GLOBALSETTINGS_TOGGLE = BASE_PATH + nameof(GLOBALSETTINGS_TOGGLE);
                public const string GLOBALSETTINGS_SET = BASE_PATH + nameof(GLOBALSETTINGS_SET);
                public const string SHUTDOWN = BASE_PATH + nameof(SHUTDOWN);
                public const string SUDOCOMMAND = BASE_PATH + nameof(SUDOCOMMAND);
                public const string PREFERENCES_DEFAULT = BASE_PATH + nameof(PREFERENCES_DEFAULT);
                public const string PREFERENCES_TOGGLE = BASE_PATH + nameof(PREFERENCES_TOGGLE);
                public const string PREFERENCES_SET = BASE_PATH + nameof(PREFERENCES_SET);
                public const string RELOAD = BASE_PATH + nameof(RELOAD);
                public const string RELOAD_LIST = BASE_PATH + nameof(RELOAD_LIST);
                public const string EXCEPTION = BASE_PATH + nameof(EXCEPTION);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { PING, "Replies with a pong and what the current delay is." },
                        { EDITCOMMAND_SETROLE, "Sets a list of roles required to use each command supplied" },
                        { EDITCOMMAND_SETPERM, "Sets a permission required to use each command supplied" },
                        { EDITCOMMAND_RESET, "Resets the roles and permissions required to use each command supplied" },
                        { EDITCOMMAND_BLACKLIST, "Prevents anyone with permissions below the override permissions from using the command in the given channel" },
                        { SETTINGS_DEFAULT, "Lists all settings available" },
                        { SETTINGS_TOGGLE, "Toggles the given setting. Only works for true/false/yes/no settings" },
                        { SETTINGS_SET, "Sets the given setting to the given value." },
                        { SETTINGSRESET_THISGUILD, "Resets settings for this guild" },
                        { SETTINGSRESET_GIVENGUILD, "Resets the given guild" },
                        { ABOUT, "Shows some about text for me" },
                        { DONATE, "Gives you the link you awesome person :heart:" },
                        { HELP, "Displays a list of all commands, or help for a single command" },
                        { HELP_TUTORIAL, "Displays information about a given tutorial" },
                        { INFO_LANGUAGE, "Displays info about what languages are supported" },
                        { INFO_TECHNICAL, "Displays technical info for me" },
                        { INVITE, "Shows the invite link" },
                        { PREFIX_SHOW, "Gets all the available current prefixes" },
                        { PREFIX_SET, "Sets the custom prefix" },
                        { DBPURGE, "Wipes the given table." },
                        { EXEC, "Executes arbitrary code" },
                        { GLOBALSETTINGS_DEFAULT, "Lists all settings available" },
                        { GLOBALSETTINGS_TOGGLE, "Toggles the given setting. Only works for true/false/yes/no settings" },
                        { GLOBALSETTINGS_SET, "Sets the given setting to the given value." },
                        { SHUTDOWN, "Shuts me down now, or in the time provided" },
                        { SUDOCOMMAND, "Will execute any command as the given user" },
                        { PREFERENCES_DEFAULT, "Lists all preferences that are available" },
                        { PREFERENCES_TOGGLE, "Toggles the given preference. Only works for true/false/yes/no preferences" },
                        { PREFERENCES_SET, "Sets the given preference to the given value" },
                        { RELOAD, "Reloads the supplied area" },
                        { RELOAD_LIST, "Lists all available areas to reload" },
                        { EXCEPTION, "Shows details about the given exception" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
