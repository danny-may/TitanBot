using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class InviteCommand : Command
    {
        public InviteCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Description = "Provides a link to invite me to any guild";
            Usage.Add("`{0}` - Shows the invite link");
            Calls.AddNew(a => GetInviteAsync());
        }

        protected async Task GetInviteAsync()
        {
            await ReplyAsync($"Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot&permissions={Configuration.Instance.InvitePermissions}>", ReplyType.Error);
        }
    }
}
