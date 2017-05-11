using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class SetCommand : Command
    {
        public SetCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => SetAvatarAsync((Uri)a[0]))
                 .WithArgTypes(typeof(Uri))
                 .WithSubCommand("Avatar");
            Calls.AddNew(a => SetGameAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0)
                 .WithSubCommand("Game");
            Calls.AddNew(a => SetGameAsync(null))
                 .WithSubCommand("Game");
            RequireOwner = true;
            Usage.Add("`{0} avatar <avatarUrl>` - Sets my avatar.");
            Usage.Add("`{0} game <game>` - Sets my game.");
            Description = "Used to set various attributes about me";
        }

        private async Task SetAvatarAsync(Uri location)
        {
            var msg = await ReplyAsync("Updating avatar...", ReplyType.Success);
            var imgBytes = await Context.WebClient.GetBytes(location, 0);
            var image = new Discord.Image(new MemoryStream(imgBytes));

            await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = image);
            
            await msg.DeleteAsync();
            await ReplyAsync("Avatar updated!", ReplyType.Success);
        }

        private async Task SetGameAsync(string game)
        {
            game = game ?? "";
            await Context.Client.SetGameAsync(game);
            await ReplyAsync("Set my game successfully!");
        }
    }
}
