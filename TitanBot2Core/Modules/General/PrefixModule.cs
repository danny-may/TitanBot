using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.General
{
    public partial class GeneralModule
    {
        [Group("Prefix")]
        [Summary("Gets and sets the prefixes for this bot")]
        public class PrefixModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Gets the current prefixes")]
            [RequireCustomPermission(0)]
            public async Task GetPrefixAsync()
            {
                var prefixes = await Context.GetPrefixes();

                if (prefixes.Length > 0)
                    await ReplyAsync($"{Res.Str.InfoText} Your available prefixes are {string.Join(", ", prefixes)}");
                else
                    await ReplyAsync($"{Res.Str.InfoText} You do not require prefixes in this channel");
            }

            [Command(RunMode = RunMode.Async)]
            [RequireCustomPermission(8)]
            [RequireContext(ContextType.Guild)]
            [Remarks("Sets the guilds prefix to the given value")]
            public async Task SetPrefixAsync(string prefix)
            {
                var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
                guildData.Prefix = prefix;
                await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData));
                await ReplyAsync($"{Res.Str.SuccessText} Your guilds prefix has been set to `{prefix}`");
            }
        }

    }
}
