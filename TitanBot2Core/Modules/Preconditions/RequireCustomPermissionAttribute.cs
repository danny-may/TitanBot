using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;
using TitanBot2.Database;
using System.Linq;
using TitanBot2.Extensions;

namespace TitanBot2.Modules.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
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
            var owner = (await context.Client.GetApplicationInfoAsync()).Owner.Id;

            if (context.User.Id == owner || context.Channel is IDMChannel || context.Channel is IGroupChannel)
                return PreconditionResult.FromSuccess();

            if (context.Guild.OwnerId == context.User.Id)
                return PreconditionResult.FromSuccess();

            var cmdPerm = await TitanbotDatabase.CmdPerms.GetCmdPerm(context.Guild.Id, command.Name);
            var guildUser = context.User as IGuildUser;

            if (guildUser.RoleIds.Any(r => cmdPerm?.roleIds?.Contains(r) ?? false) ||
                guildUser.HasAll(cmdPerm?.permissionId ?? DefaultPerm))
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError("You do not have the required permissions to access this command");
                
        }
    }
}
