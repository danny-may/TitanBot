using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    [Description("Used to set various attributes about me")]
    [RequireOwner]
    class SetCommand : Command
    {
        [Call("Avatar")]
        [Usage("Sets my avatar.")]
        async Task SetAvatarAsync(Uri location = null)
        {
            if (location == null && Context.Message.Attachments.Count > 0)
                location = new Uri(Context.Message.Attachments.First().Url);
            else if (location == null)
            {
                await ReplyAsync("You must provide an image as either an attachment or URL!", ReplyType.Error);
                return;
            }
            var msg = await ReplyAsync("Updating avatar...", ReplyType.Success);
            var imgBytes = await Context.WebClient.GetBytes(location, 0);
            var image = new Discord.Image(new MemoryStream(imgBytes));

            await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = image);
            
            await msg.DeleteAsync();
            await ReplyAsync("Avatar updated!", ReplyType.Success);
        }

        [Call("Game")]
        [Usage("Sets my game")]
        async Task SetGameAsync(string game = null)
        {
            game = game ?? "";
            await Context.Client.SetGameAsync(game);
            await ReplyAsync("Set my game successfully!");
        }
    }
}
