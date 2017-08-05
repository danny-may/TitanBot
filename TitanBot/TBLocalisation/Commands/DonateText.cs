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
            public static class DonateText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "DONATE_";

                public const string MESSAGE_ADDITIONAL = BASE_PATH + nameof(MESSAGE_ADDITIONAL);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { MESSAGE_ADDITIONAL, "" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
