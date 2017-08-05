using System.Collections.Generic;
using System.Collections.Immutable;

namespace TitanBot
{
    public static partial class TBLocalisation
    {
        public static partial class Commands
        {
            public static class PingText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "PING_";
                
                public const string INITIAL = BASE_PATH + nameof(INITIAL);
                public const string VERIFY = BASE_PATH + nameof(VERIFY);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { INITIAL, "| ~{0} ms" },
                        { VERIFY, "| {0} ms" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
