using System.Collections.Generic;
using System.Linq;
using TitanBot.Commands;
using System.Collections.Immutable;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public const string REPLYTYPE_SUCCESS = nameof(REPLYTYPE_SUCCESS);
        public const string REPLYTYPE_ERROR = nameof(REPLYTYPE_ERROR);
        public const string REPLYTYPE_INFO = nameof(REPLYTYPE_INFO);
        public const string NOTES = nameof(NOTES);
        public const string GROUP = nameof(GROUP);
        public const string ALIASES = nameof(ALIASES);
        public const string USAGE = nameof(USAGE);
        public const string FLAGS = nameof(FLAGS);
        public const string INPUT = nameof(INPUT);
        public const string ERROR = nameof(ERROR);
        public const string OUTPUT = nameof(OUTPUT);
        public const string UNKNOWNUSER = nameof(UNKNOWNUSER);

        public static IReadOnlyDictionary<string, string> Defaults { get; }
            = new Dictionary<string, string>
            {
                { REPLYTYPE_SUCCESS, ":white_check_mark: **Got it!** " },
                { REPLYTYPE_ERROR, ":no_entry_sign: **Oops!** " },
                { REPLYTYPE_INFO, ":information_source: " },
                { NOTES, "Notes" },
                { GROUP, "Group" },
                { ALIASES, "Aliases" },
                { USAGE, "Usage" },
                { FLAGS, "Flags" },
                { INPUT, "Input" },
                { ERROR, "Error" },
                { OUTPUT, "Output" },
                { UNKNOWNUSER, "UNKNOWNUSER#0000" }

            }.Concat(Help.Defaults)
             .Concat(Settings.Defaults)
             .Concat(Logic.Defaults)
             .Concat(Types.Defaults)
             .Concat(Commands.Defaults)
             .ToImmutableDictionary();
    }
}