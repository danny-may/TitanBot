using System.Linq;
using System.Threading.Tasks;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("SETTINGSRESET_HELP_DESCRIPTION")]
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
        [Usage("SETTINGSRESET_HELP_USAGE_THISGUILD")]
        async Task ResetGuild()
        {
            await ResetGuild(Guild.Id);
        }

        [Call]
        [Usage("SETTINGSRESET_HELP_USAGE_GIVENGUILD")]
        [RequireOwner]
        async Task ResetGuild(ulong guildId)
        {
            var calls = CommandService.CommandList.SelectMany(c => c.Calls).ToArray();
            PermissionManager.ResetPermissions(Context, calls);
            SettingsManager.ResetSettings(Context.Guild.Id);
            var guild = Client.GetGuild(guildId);
            if (guild == null)
                await ReplyAsync("SETTINGRESET_GUILD_NOTEXIST", ReplyType.Error);
            else
                await ReplyAsync("SETTINGRESET_SUCCESS", ReplyType.Success, guild.Name, guild.Id);
        }
    }
}
