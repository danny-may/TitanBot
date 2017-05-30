using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.General
{
    [Description("Provides a link to invite me to any guild")]
    class InviteCommand : Command
    {
        [Call]
        [Usage("Shows the invite link")]
        async Task GetInviteAsync()
        {
            await ReplyAsync($"Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot&permissions={Configuration.Instance.InvitePermissions}>", ReplyType.Success);
        }
    }
}
