using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules
{

    [Name("Prefix")]
    public class PrefixModule : TitanBotModule
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
        [RequireCustomPermission(8)]
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
