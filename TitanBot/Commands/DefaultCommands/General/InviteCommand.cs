using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description(Desc.INVITE)]
    public class InviteCommand : Command
    {
        [Call]
        [Usage(Usage.INVITE)]
        async Task GetInviteAsync()
        {
            await ReplyAsync(InviteText.MESSAGE, ReplyType.Info, BotUser.Id, GeneralGlobalSetting.PreferredPermission);
        }
    }
}
