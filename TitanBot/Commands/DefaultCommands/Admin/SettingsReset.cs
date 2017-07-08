using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Settings;
using TitanBot.Storage.Tables;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("Resets all settings and command permissions for a guild.")]
    class SettingsReset : Command
    {
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
            var success = await Database.Delete<Setting>(guildId);
            await Database.Delete<CallPermission>(p => p.GuildId == guildId);
            var guild = Client.GetGuild(guildId);
            if (guild == null)
                await ReplyAsync("That guild does not exist.", ReplyType.Error);
            else
                await ReplyAsync($"All settings deleted for {guild.Name}({guildId})", ReplyType.Success);
        }
    }
}
