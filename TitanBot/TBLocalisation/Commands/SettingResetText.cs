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
            public static class SettingResetText
            {
                private const string BASE_PATH = Commands.BASE_PATH + "SETTINGRESET_";

                public const string GUILD_NOTEXIST = BASE_PATH + nameof(GUILD_NOTEXIST);
                public const string SUCCESS = BASE_PATH + nameof(SUCCESS);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { GUILD_NOTEXIST, "That guild does not exist." },
                        { SUCCESS, "All settings deleted for {0}({1})" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
