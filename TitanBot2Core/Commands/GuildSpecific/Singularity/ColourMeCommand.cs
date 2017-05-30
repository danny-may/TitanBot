using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.GuildSpecific.Singularity
{
    public class ColourMeCommand : Command
    {
        public ColourMeCommand()
        {
            GuildRestrictions = new ulong[] { 307803032534646785 };

            //Handlers.AddNew(a => ColourRole((System.Drawing.Color)a[0]))
            //     .WithItemAsParams(0);
        }

        private ulong _dailyChamp = 314036070100500493;
        private ulong _weeklyChamp = 314036021975318538;

        private async Task ColourRole(System.Drawing.Color color)
        {
            var user = Context.User as SocketGuildUser;
            if (user == null)
            {
                await ReplyAsync("An error occured, please try again later.", ReplyType.Error);
                return;
            }

            SocketRole target;
            target = user.Roles.SingleOrDefault(r => r.Id == _dailyChamp);
            if (target == null)
                target = user.Roles.SingleOrDefault(r => r.Id == _weeklyChamp);
            if (target == null)
            {
                await ReplyAsync("You cannot change the colour of this role!", ReplyType.Error);
                return;
            }

            await target.ModifyAsync(r => r.Color = color.ToDiscord());
            await ReplyAsync($"You have changed the colour of the {target.Mention} role");
        }
    }
}
