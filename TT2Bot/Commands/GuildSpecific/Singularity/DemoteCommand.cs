using Discord;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TT2Bot.Commands.GuildSpecific.Singularity
{
    [Description("Demotes a user down the ranks, potentially even out of the clan")]
    [RequireGuild(307803032534646785)]
    [DefaultPermission(8)]
    class DemoteCommand : Command
    {
        private ulong[] _roleOrder = new ulong[]
        {
            307806171798962177,
            307805963295916032,
            307806447402614785,
            307806702151925760,
            307807022903197698,
            312177555379585024,
        };

        [Call]
        [Usage("Demotes the given user")]
        async Task Demote([Dense]IGuildUser target)
        {
            var callingUser = GuildAuthor;
            if (callingUser == null)
            {
                await ReplyAsync("An error occured, please try again later.", ReplyType.Error);
                return;
            }

            var userRoles = Guild.Roles.Where(r => callingUser.RoleIds.Contains(r.Id)).ToArray();
            var targetRoles = Guild.Roles.Where(r => target.RoleIds.Contains(r.Id)).ToArray();

            var userTopRole = userRoles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));
            var targetTopRole = targetRoles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));

            if (userTopRole == null)
            {
                await ReplyAsync("You do not have a member rank here!", ReplyType.Error);
                return;
            }

            if (targetTopRole == null)
            {
                await ReplyAsync("They do not have a member rank here! Please give them one of the member roles to use this command on them.", ReplyType.Error);
                return;
            }

            if (userTopRole.Position <= targetTopRole.Position)
            {
                await ReplyAsync("You cannot demote someone who is above or equal to you", ReplyType.Error);
                return;
            }

            var roles = Guild.Roles.Where(r => _roleOrder.Contains(r.Id)).OrderBy(r => r.Position);
            var demoRole = roles.LastOrDefault(r => r.Position < targetTopRole.Position);

            if (demoRole == null)
            {
                await ReplyAsync("There is no lower rank for me to demote them to!", ReplyType.Error);
                return;
            }

            await target.RemoveRolesAsync(roles.Take(roles.Count() - 1).Where(r => target.RoleIds.Contains(r.Id)));
            await target.AddRoleAsync(demoRole);

            await ReplyAsync($"Demoted {target.Username} to {demoRole.Name}");
        }
    }
}

