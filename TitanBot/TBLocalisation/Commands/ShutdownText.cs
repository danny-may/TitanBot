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
            public static class ShutdownText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "SHUTDOWN_";

                public const string INTIME = BASE_PATH + nameof(INTIME);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { INTIME, "Shutting down in {0}" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
