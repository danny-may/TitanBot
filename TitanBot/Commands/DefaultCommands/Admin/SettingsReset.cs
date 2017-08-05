using System.Linq;
using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Help;
using static TitanBot.TBLocalisation.Commands;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description(Desc.SETTINGSRESET)]
    public class SettingsReset : Command
    {
        IPermissionManager PermissionManager { get; }

        public SettingsReset(IPermissionManager permissionManager)
        {
            PermissionManager = permissionManager;
        }

        [DefaultPermission(8)]
        [Call]
        [RequireContext(ContextType.Guild)]
        [Usage(Usage.SETTINGSRESET_THISGUILD)]
        async Task ResetGuild()
        {
            await ResetGuild(Guild.Id);
        }

        [Call]
        [Usage(Usage.SETTINGSRESET_GIVENGUILD)]
        [RequireOwner]
        async Task ResetGuild(ulong guildId)
        {
            var calls = CommandService.CommandList.SelectMany(c => c.Calls).ToArray();
            PermissionManager.ResetPermissions(Context, calls);
            SettingsManager.ResetSettings(Context.Guild);
            var guild = Client.GetGuild(guildId);
            if (guild == null)
                await ReplyAsync(SettingResetText.GUILD_NOTEXIST, ReplyType.Error);
            else
                await ReplyAsync(SettingResetText.SUCCESS, ReplyType.Success, guild.Name, guild.Id);
        }
    }
}
