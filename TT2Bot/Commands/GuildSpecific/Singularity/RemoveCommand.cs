using Discord;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TT2Bot.Commands.GuildSpecific.Singularity
{
    [Description("Removes a user completely from the guild")]
    [RequireGuild(307803032534646785)]
    class RemoveCommand : Command
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
        [Usage("Removes the given user from the clan with the given reason")]
        async Task Remove(IGuildUser target, [Dense]string reason)
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
                await ReplyAsync("You cannot remove someone who is above or equal to you", ReplyType.Error);
                return;
            }

            await target.RemoveRolesAsync(targetRoles.Where(r => _roleOrder.Contains(r.Id)));

            await ReplyAsync(target, "You have been kicked from the clan for the following reason:\n```" + reason + "```\nWe hope you enjoyed your stay and wish you luck in your future clans!");

            await ReplyAsync($"Removed {target.Mention} from the clan.", ReplyType.Success);
        }
    }
}
