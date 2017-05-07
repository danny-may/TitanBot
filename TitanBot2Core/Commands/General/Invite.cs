using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class Invite : Command
    {
        public Invite(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Description = "Provides a link to invite me to any guild";
            Usage = new string[]
            {
                "`{0}` - Shows the invite link"
            };
        }

        protected override async Task RunAsync()
        {
            await ReplyAsync($"{Res.Str.InfoText} Want to invite me to your guild? Click this link!\n<https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&scope=bot&permissions={Configuration.Instance.InvitePermissions}>");
        }
    }
}
