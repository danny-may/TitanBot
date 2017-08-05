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
            public static class PrefixText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "PREFIX_";

                public const string SHOW_NOPREFIX = BASE_PATH + nameof(SHOW_NOPREFIX);
                public const string SHOW_MESSAGE = BASE_PATH + nameof(SHOW_MESSAGE);
                public const string SET_MESSAGE = BASE_PATH + nameof(SET_MESSAGE);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { SHOW_NOPREFIX, "You do not require prefixes in this channel" },
                        { SHOW_MESSAGE, "Your available prefixes are {0)}" },
                        { SET_MESSAGE, "Your guilds prefix has been set to `{0}`" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
