using System.Threading.Tasks;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("INVITE_HELP_DESCRIPTION")]
    public class InviteCommand : Command
    {
        [Call]
        [Usage("INVITE_HELP_USAGE")]
        async Task GetInviteAsync()
        {
            await ReplyAsync("INVITE_MESSAGE", ReplyType.Info, BotUser.Id, GlobalSettings.PreferredPermission);
        }
    }
}
