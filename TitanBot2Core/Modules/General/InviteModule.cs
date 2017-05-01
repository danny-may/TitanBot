using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Modules;
using TitanBot2.Preconditions;

namespace TitanBot2Core.Modules.General
{
    public partial class General
    {
        [Group("Invite")]
        [Summary("Gets an invite link to get Titanbot to join your guild too.")]
        [RequireCustomPermission(0)]
        public class InviteModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            public async Task InviteAsync()
            {
                await ReplyAsync($"{Res.Str.InfoText} Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot&permissions={Configuration.Instance.InvitePermissions}>");
            }
        }
    }
}
