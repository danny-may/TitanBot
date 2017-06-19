using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;

namespace TT2Bot.Commands.GuildSpecific.Singularity
{
    [Description("Sets your colour, if you have the daily or weekly champion role")]
    [RequireGuild(307803032534646785)]
    class ColourMeCommand : Command
    {
        private ulong _dailyChamp = 314036070100500493;
        private ulong _weeklyChamp = 314036021975318538;

        [Call]
        [Usage("Sets your colour to whatever you want")]
        async Task ColourRole([Dense]System.Drawing.Color color)
        {
            var user = Author as IGuildUser;
            if (user == null)
            {
                await ReplyAsync("An error occured, please try again later.", ReplyType.Error);
                return;
            }

            var target = user.RoleIds.SingleOrDefault(r => r == _dailyChamp);
            if (target == 0)
                target = user.RoleIds.SingleOrDefault(r => r == _weeklyChamp);
            if (target == 0)
            {
                await ReplyAsync("You cannot change the colour of this role!", ReplyType.Error);
                return;
            }

            var role = Guild.Roles.First(r => r.Id == target);

            await role.ModifyAsync(r => r.Color = color.ToDiscord());
            await ReplyAsync($"You have changed the colour of the {role.Mention} role");
        }
    }
}
