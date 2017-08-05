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
            public static class Notes
            {
                private const string BASE_PATH = Help.BASE_PATH + "NOTES_";
                
                public const string EDITCOMMAND = BASE_PATH + nameof(EDITCOMMAND);
                public const string EXEC = BASE_PATH + nameof(EXEC);

                public static IReadOnlyDictionary<string, string> Defaults { get; }
                    = new Dictionary<string, string>
                    {
                        { EDITCOMMAND, "To work out just what permission id you need, give the [permission calculator](https://discordapi.com/permissions.html) a try!" },
                        { EXEC, "https://github.com/Titansmasher/TitanBot/blob/rewrite/TitanBotBase/Commands/DefaultCommands/Owner/ExecCommand.cs#L111" }
                    }.ToImmutableDictionary();
            }
        }
    }
}
