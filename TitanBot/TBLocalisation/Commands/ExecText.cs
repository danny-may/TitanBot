using System.Collections.Generic;
using System.Collections.Immutable;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static partial class Commands
        {
            public static class ExecText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "EXEC_";

                public const string FOOTER_CONSTRUCTFAILED = BASE_PATH + nameof(FOOTER_CONSTRUCTFAILED);
                public const string FOOTER_COMPILEFAILED = BASE_PATH + nameof(FOOTER_COMPILEFAILED);
                public const string FOOTER_EXECUTEFAILED = BASE_PATH + nameof(FOOTER_EXECUTEFAILED);
                public const string FOOTER_SUCCESS = BASE_PATH + nameof(FOOTER_SUCCESS);
                public const string INPUT_FORMAT = BASE_PATH + nameof(INPUT_FORMAT);
                public const string OUTPUT_NULL = BASE_PATH + nameof(OUTPUT_NULL);
                public const string OUTPUT_FORMAT = BASE_PATH + nameof(OUTPUT_FORMAT);
                public const string TITLE_EXCEPTION = BASE_PATH + nameof(TITLE_EXCEPTION);
                public const string TITLE_SUCCESS = BASE_PATH + nameof(TITLE_SUCCESS);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { FOOTER_CONSTRUCTFAILED, "Failed to construct. Took {0}ms" },
                        { FOOTER_COMPILEFAILED, "Constructed in: {0}ms | Failed to compile. Took {1}ms" },
                        { FOOTER_EXECUTEFAILED, "Constructed in: {0}ms | Compiled in: {1}ms | Failed to execute. Took {2}ms" },
                        { FOOTER_SUCCESS, "Constructed in: {0}ms | Compiled in: {1}ms | Executed in: {2}ms" },
                        { INPUT_FORMAT, "```csharp\n{0}\n```" },
                        { OUTPUT_NULL, "No output from execution..." },
                        { OUTPUT_FORMAT, "Type: {0}\n```csharp\n{1}\n```" },
                        { TITLE_EXCEPTION, ":no_entry_sign: Execution Result" },
                        { TITLE_SUCCESS, ":white_check_mark: Execution Result" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
