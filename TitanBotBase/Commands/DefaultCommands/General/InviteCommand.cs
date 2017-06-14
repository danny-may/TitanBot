using System.Threading.Tasks;

namespace TitanBotBase.Commands.DefautlCommands.General
{
    [Description("Provides a link to invite me to any guild")]
    class InviteCommand : Command
    {
        [Call]
        [Usage("Shows the invite link")]
        async Task GetInviteAsync()
        {
            await ReplyAsync($"Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={BotUser.Id}&scope=bot&permissions=8>", ReplyType.Success);
        }
    }
}
