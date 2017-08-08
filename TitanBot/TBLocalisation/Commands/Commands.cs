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
            private const string BASE_PATH = "COMMANDS_";

            public static IReadOnlyDictionary<string, string> Defaults { get; }
                = new Dictionary<string, string>().Concat(PingText.Defaults)
                                                  .Concat(InfoText.Defaults)
                                                  .Concat(EditCommandText.Defaults)
                                                  .Concat(SettingText.Defaults)
                                                  .Concat(SettingResetText.Defaults)
                                                  .Concat(AboutText.Defaults)
                                                  .Concat(DonateText.Defaults)
                                                  .Concat(HelpText.Defaults)
                                                  .Concat(InviteText.Defaults)
                                                  .Concat(PrefixText.Defaults)
                                                  .Concat(DbPurgeText.Defaults)
                                                  .Concat(ExecText.Defaults)
                                                  .Concat(ShutdownText.Defaults)
                                                  .Concat(SudoText.Defaults)
                                                  .Concat(ExceptionText.Defaults)
                                                  .Concat(LanguageText.Defaults)
                                                  .ToImmutableDictionary();
        }
    }
}
