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
            public static class EditCommandText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "EDITCOMMAND_";

                public const string FINDCALLS_NORESULTS = BASE_PATH + nameof(FINDCALLS_NORESULTS);
                public const string SUCCESS = BASE_PATH + nameof(SUCCESS);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { FINDCALLS_NORESULTS, "There were no commands that matched those calls." },
                        { SUCCESS, "{0} set successfully for {1} command(s)!" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
