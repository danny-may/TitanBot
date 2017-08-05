using System.Threading.Tasks;
using TitanBot.Replying;
using TitanBot.Settings;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(Desc.PREFIX)]
    [DefaultPermission(8)]
    public class PrefixCommand : Command
    {
        [Call]
        [DefaultPermission(0, "Show")]
        [Usage(Usage.PREFIX_SHOW)]
        async Task GetPrefixesAsync()
        {
            if (AcceptedPrefixes.Length > 0)
                await ReplyAsync(PrefixText.SHOW_MESSAGE, ReplyType.Info, string.Join(", ", AcceptedPrefixes));
            else
                await ReplyAsync(PrefixText.SHOW_NOPREFIX, ReplyType.Info);
        }

        [Call]
        [DefaultPermission(8, "Set")]
        [RequireContext(ContextType.Guild)]
        [Usage(Usage.PREFIX_SET)]
        async Task SetPrefixAsync(string newPrefix)
        {
            GuildSettings.Edit<GeneralGuildSetting>(s => s.Prefix = newPrefix.ToLower());
            await ReplyAsync(PrefixText.SET_MESSAGE, ReplyType.Success, newPrefix.ToLower());
        }
    }
}
