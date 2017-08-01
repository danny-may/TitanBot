using System.Threading.Tasks;
using TitanBot.Replying;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(TitanBotResource.INVITE_HELP_DESCRIPTION)]
    public class InviteCommand : Command
    {
        [Call]
        [Usage(TitanBotResource.INVITE_HELP_USAGE)]
        async Task GetInviteAsync()
        {
            await ReplyAsync(TitanBotResource.INVITE_MESSAGE, ReplyType.Info, BotUser.Id, GeneralGlobalSetting.PreferredPermission);
        }
    }
}
