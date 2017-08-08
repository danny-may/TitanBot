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
            public static class FormattingText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "FORMATTING_";

                public const string LIST_TITLE = BASE_PATH + nameof(LIST_TITLE);
                public const string SET_SUCCESS = BASE_PATH + nameof(SET_SUCCESS);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { LIST_TITLE, "Here are all the formats available to you" },
                        { SET_SUCCESS, "Your output format has been set to {0}" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
