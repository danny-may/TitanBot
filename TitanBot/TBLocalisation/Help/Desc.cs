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
            public static class Desc
            {
                private const string BASE_PATH = Help.BASE_PATH + "DESCRIPTION_";

                public const string PING = BASE_PATH + nameof(PING);
                public const string EDITCOMMAND = BASE_PATH + nameof(EDITCOMMAND);
                public const string SETTINGS = BASE_PATH + nameof(SETTINGS);
                public const string SETTINGSRESET = BASE_PATH + nameof(SETTINGSRESET);
                public const string ABOUT = BASE_PATH + nameof(ABOUT);
                public const string DONATE = BASE_PATH + nameof(DONATE);
                public const string HELP = BASE_PATH + nameof(HELP);
                public const string INFO = BASE_PATH + nameof(INFO);
                public const string INVITE = BASE_PATH + nameof(INVITE);
                public const string DBPURGE = BASE_PATH + nameof(DBPURGE);
                public const string PREFIX = BASE_PATH + nameof(PREFIX);
                public const string EXEC = BASE_PATH + nameof(EXEC);
                public const string GLOBALSETTINGS = BASE_PATH + nameof(GLOBALSETTINGS);
                public const string SHUTDOWN = BASE_PATH + nameof(SHUTDOWN);
                public const string SUDOCOMMAND = BASE_PATH + nameof(SUDOCOMMAND);
                public const string PREFERENCES = BASE_PATH + nameof(PREFERENCES);
                public const string EXCEPTION = BASE_PATH + nameof(EXCEPTION);
                public const string LANGUAGE = BASE_PATH + nameof(LANGUAGE);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { PING, "Basic command for calculating my delay." },
                        { EDITCOMMAND, "Used to allow people with varying roles or permissions to use different commands." },
                        { SETTINGS, "Allows the retrieval and changing of existing settings for the server" },
                        { SETTINGSRESET, "Resets all settings and command permissions for a guild." },
                        { ABOUT, "A tiny command that just displays some helpful links :)" },
                        { DONATE, "You.. youre thinking of donating? :open_mouth: This command will give you a link to my patreon page" },
                        { HELP, "Displays help for any command" },
                        { INFO, "Displays some technical information about me" },
                        { INVITE, "Provides a link to invite me to any guild" },
                        { PREFIX, "Gets or sets a custom prefix that is required to use my commands" },
                        { DBPURGE, "Wipes any database table clean" },
                        { EXEC, "Allows for arbitrary code execution" },
                        { GLOBALSETTINGS, "Allows the retrieval and changing of existing settings for the server" },
                        { SHUTDOWN, "Shuts me down" },
                        { SUDOCOMMAND, "Executes any command on behalf of another user. Requires a prefix in the message" },
                        { PREFERENCES, "Allows you to set some options for how I will interact with you." },
                        { EXCEPTION, "Shows details about exceptions that have occured within me" },
                        { LANGUAGE, "Shows information about any and all languages supported by me" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
