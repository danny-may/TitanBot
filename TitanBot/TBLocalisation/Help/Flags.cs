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
            public static class Flags
            {
                private const string BASE_PATH = Help.BASE_PATH + "FLAG_";

                public const string EXCEPTION_F = BASE_PATH + nameof(EXCEPTION_F);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { EXCEPTION_F, "Returns the full exception in a file" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
