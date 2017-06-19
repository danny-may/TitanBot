using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TT2Bot.Commands.GuildSpecific.Singularity
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
            var targetRole = Guild.GetRole(roleId);
            var users = (await Guild.GetUsersAsync()).Where(u => u.RoleIds.Contains(targetRole.Id));
            foreach (var user in users)
                await user.RemoveRoleAsync(targetRole);
            await target.AddRoleAsync(targetRole);

            await ReplyAsync($"Congratulations {target.Mention}! You have been given the {targetRole.Mention} role!");
        }
    }
}
