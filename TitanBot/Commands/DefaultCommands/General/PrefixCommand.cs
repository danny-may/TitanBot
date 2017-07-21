using System.Threading.Tasks;
using TitanBot.Settings;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("PREFIX_HELP_DESCRIPTION")]
    [DefaultPermission(8)]
    public class PrefixCommand : Command
    {
        [Call]
        [DefaultPermission(0, "Show")]
        [Usage("PREFIX_HELP_USAGE_SHOW")]
        async Task GetPrefixesAsync()
        {
            if (AcceptedPrefixes.Length > 0)
                await ReplyAsync("PREFIX_SHOW_MESSAGE", ReplyType.Info, string.Join(", ", AcceptedPrefixes));
            else
                await ReplyAsync("PREFIX_SHOW_NOPREFIX", ReplyType.Info);
        }

        [Call]
        [DefaultPermission(8, "Set")]
        [RequireContext(ContextType.Guild)]
        [Usage("PREFIX_HELP_USAGE_SET")]
        async Task SetPrefixAsync(string newPrefix)
        {
            GuildSettings.Edit<GeneralGuildSetting>(s => s.Prefix = newPrefix.ToLower());
            await ReplyAsync("PREFIX_SET_MESSAGE", ReplyType.Success, newPrefix.ToLower());
        }
    }
}
