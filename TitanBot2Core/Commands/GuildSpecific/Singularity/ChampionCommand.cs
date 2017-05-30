using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.GuildSpecific.Singularity
{
    [Description("Sets a person as the clan champion for the day or week")]
    [RequireGuild(307803032534646785)]
    [DefaultPermission(8)]
    class ChampionCommand : Command
    {
        [Call("Daily")]
        [Usage("Sets the champion for today")]
        Task SetDailyAsync([Dense]SocketGuildUser user)
            => SetChampAsync(user, 314036070100500493);

        [Call("Weekly")]
        [Usage("Sets the champion for this week")]
        Task SetWeeklyAsync([Dense]SocketGuildUser user)
            => SetChampAsync(user, 314036021975318538);

        async Task SetChampAsync(SocketGuildUser target, ulong roleId)
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
