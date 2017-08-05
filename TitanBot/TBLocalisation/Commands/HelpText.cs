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
            public static class HelpText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "HELP_";

                public const string LIST_TITLE = BASE_PATH + nameof(LIST_TITLE);
                public const string LIST_DESCRIPTION = BASE_PATH + nameof(LIST_DESCRIPTION);
                public const string LIST_COMMAND = BASE_PATH + nameof(LIST_COMMAND);
                public const string SINGLE_UNRECOGNISED = BASE_PATH + nameof(SINGLE_UNRECOGNISED);
                public const string SINGLE_NOUSAGE = BASE_PATH + nameof(SINGLE_NOUSAGE);
                public const string SINGLE_NODESCRIPTION = BASE_PATH + nameof(SINGLE_NODESCRIPTION);
                public const string SINGLE_NOGROUP = BASE_PATH + nameof(SINGLE_NOGROUP);
                public const string SINGLE_TITLE = BASE_PATH + nameof(SINGLE_TITLE);
                public const string SINGLE_USAGE_FOOTER = BASE_PATH + nameof(SINGLE_USAGE_FOOTER);
                public const string SINGLE_FOOTER = BASE_PATH + nameof(SINGLE_FOOTER);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { LIST_TITLE, "These are the commands you can use" },
                        { LIST_DESCRIPTION, "To use a command, type `<prefix><command>`\n  e.g. `{0}help`\n" +
                                                            "To pass arguments, add a list of values after the command separated by space\n  e.g. `{0}command a b c, d`\n" +
                                                            "You can use any of these prefixes: \"{1}\"" },
                        { LIST_COMMAND, "{0} Commands:\n    {1}" },
                        { SINGLE_UNRECOGNISED, "`{0}` is not a recognised command. Use `{1}help` for a list of all available commands" },
                        { SINGLE_NOUSAGE, "No usage available!" },
                        { SINGLE_NODESCRIPTION, "No description!" },
                        { SINGLE_NOGROUP, "No categories!" },
                        { SINGLE_TITLE, "Help for {0}" },
                        { SINGLE_USAGE_FOOTER, "\n\n_`<param>` = required\n`[param]` = optional\n`<param...>` = accepts multiple (comma separated)_" },
                        { SINGLE_FOOTER, "{0} | Help"}
                    }.ToImmutableDictionary();
            }
        }
    }
}
