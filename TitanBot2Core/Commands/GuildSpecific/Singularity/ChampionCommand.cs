using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.GuildSpecific.Singularity
{
    public class ChampionCommand : Command
    {
        public ChampionCommand(CmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            GuildRestrictions = new ulong[] { 307803032534646785 };

            Calls.AddNew(a => SetChampAsync((SocketGuildUser)a[0], _dailyChamp))
                 .WithArgTypes(typeof(SocketGuildUser))
                 .WithSubCommand("Daily")
                 .WithItemAsParams(0);
            Calls.AddNew(a => SetChampAsync((SocketGuildUser)a[0], _weeklyChamp))
                 .WithArgTypes(typeof(SocketGuildUser))
                 .WithSubCommand("Weekly")
                 .WithItemAsParams(0);

            DefaultPermission = 8;

        }

        private ulong _dailyChamp = 314036070100500493;
        private ulong _weeklyChamp = 314036021975318538;

        private async Task SetChampAsync(SocketGuildUser target, ulong roleId)
        {
            var targetRole = Context.Guild.GetRole(roleId);
            var users = Context.Guild.Users.Where(u => u.Roles.Contains(targetRole));
            foreach (var user in users)
                await user.RemoveRoleAsync(targetRole);
            await target.AddRoleAsync(targetRole);

            await ReplyAsync($"Congratulations {target.Mention}! You have been given the {targetRole.Mention} role!");
        }
    }
}
