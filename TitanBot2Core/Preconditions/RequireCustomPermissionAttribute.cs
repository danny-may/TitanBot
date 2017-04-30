using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireCustomPermissionAttribute : PreconditionAttribute
    {
        private ulong DefaultPerm { get; }

        public RequireCustomPermissionAttribute(ulong defaultPermission)
        {
            DefaultPerm = defaultPermission;
        }

        public RequireCustomPermissionAttribute() : this(0) { }

        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var owner = (context as TitanbotCmdContext)?.TitanBot?.Owner ?? (await context.Client.GetApplicationInfoAsync()).Owner;

            if (context.User.Id == owner.Id || context.Channel is IDMChannel || context.Channel is IGroupChannel)
                return PreconditionResult.FromSuccess();

            if (context.Guild.OwnerId == context.User.Id)
                return PreconditionResult.FromSuccess();

            var guildData = await (context as TitanbotCmdContext).Database.Guilds.GetGuild(context.Guild.Id);
            var guildUser = context.User as IGuildUser;

            if (guildUser.HasAll(guildData.PermOverride))
                return PreconditionResult.FromSuccess();

            var cmdPerm = await (context as TitanbotCmdContext).Database.CmdPerms.GetCmdPerm(context.Guild.Id, command.Name);

            if (cmdPerm?.permissionId == null && cmdPerm?.roleIds == null)
            {
                if (guildUser.HasAll(cmdPerm?.permissionId ?? DefaultPerm))
                    return PreconditionResult.FromSuccess();
            }

            if (cmdPerm?.roleIds != null && guildUser.RoleIds.Any(r => cmdPerm?.roleIds?.Contains(r) ?? false))
                return PreconditionResult.FromSuccess();

            if (cmdPerm?.permissionId != null && guildUser.HasAll(cmdPerm?.permissionId ?? DefaultPerm))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"{context.User.Mention} You do not have the required permissions to use that command");
                
        }
    }
}
