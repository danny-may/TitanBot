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
            public static class SchedulerText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "SCHEDULER_";

                public const string PRUNED = BASE_PATH + nameof(PRUNED);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { PRUNED, "Removed {0} completed timer records" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
