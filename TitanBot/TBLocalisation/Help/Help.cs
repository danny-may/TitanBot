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
            private const string BASE_PATH = "HELP_";

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>().Concat(Desc.Defaults)
                                                  .Concat(Usage.Defaults)
                                                  .Concat(Flags.Defaults)
                                                  .Concat(Notes.Defaults)
                                                  .ToImmutableDictionary();
        }
    }
}
