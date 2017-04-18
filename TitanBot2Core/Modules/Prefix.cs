using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;

namespace TitanBot2.Modules
{
    public class Prefix : TitanBotModule
    {
        [Command("prefix")]
        public async Task GetPrefixAsync()
        {
            if (Context.Guild != null)
            {
                var guildPrefix = await TitanbotDatabase.Guilds.GetPrefix(Context.Guild.Id);
                if (guildPrefix != Configuration.Instance.Prefix)
                {
                    await ReplyAsync($"{Res.Str.InfoText} Your available prefixes are `{Configuration.Instance.Prefix}`, `{guildPrefix}`, `{Context.Client.CurrentUser.Username}` or mention me!");
                    return;
                }
            }
            await ReplyAsync($"{Res.Str.InfoText} Your available prefixes are `{Configuration.Instance.Prefix}`, `{Context.Client.CurrentUser.Username}` or mention me!");
        }

        [Command("prefix")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SetPrefixAsync(string prefix)
        {
            var guildData = await TitanbotDatabase.Guilds.GetGuild(Context.Guild.Id);
            guildData.Prefix = prefix;
            await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Update(guildData));
            await ReplyAsync($"{Res.Str.SuccessText} Your guilds prefix has been set to `{prefix}`");
        }
    }
}
