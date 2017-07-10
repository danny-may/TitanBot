using System.Linq;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("Resets all settings and command permissions for a guild.")]
    public class SettingsReset : Command
    {
        IPermissionManager PermissionManager { get; }
        ICommandContext Context { get; }

        public SettingsReset(IPermissionManager permissionManager, ICommandContext context)
        {
            PermissionManager = permissionManager;
            Context = context;
        }

        [DefaultPermission(8)]
        [Call]
        [RequireContext(ContextType.Guild)]
        [Usage("Resets settings for this guild")]
        async Task ResetGuild()
        {
            await ResetGuild(Guild.Id);
        }

        [Call]
        [Usage("Resets the given guild")]
        [RequireOwner]
        async Task ResetGuild(ulong guildId)
        {
            var calls = CommandService.CommandList.SelectMany(c => c.Calls).ToArray();
            PermissionManager.ResetPermissions(Context, calls);
            SettingsManager.ResetSettings(Context.Guild.Id);
            var guild = Client.GetGuild(guildId);
            if (guild == null)
                await ReplyAsync("That guild does not exist.", ReplyType.Error);
            else
                await ReplyAsync($"All settings deleted for {guild.Name}({guildId})", ReplyType.Success);
        }
    }
}
