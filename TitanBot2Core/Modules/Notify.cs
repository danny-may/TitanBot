using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;
using TitanBot2.Modules.Preconditions;

namespace TitanBot2.Modules
{
    [Group("notify")]
    public class Notify : TitanBotModule
    {
        [Command("alive")]
        [Alias("online")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        public async Task SetAliveAsync(IMessageChannel channel)
        {
            if (!Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
            {
                await ReplyAsync($"{Res.Str.ErrorText} That channel does not exist on this guild!");
                return;
            }

            var guildData = await TitanbotDatabase.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyAlive = channel.Id;
            await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
            await ReplyAsync($"{Res.Str.SuccessText} Set <#{channel.Id}> to recieve a message when I come online!");
            
        }

        [Command("alive")]
        [Alias("online")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        public async Task RemoveAliveAsync()
        {
            var guildData = await TitanbotDatabase.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyAlive = null;
            await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
            await ReplyAsync($"{Res.Str.SuccessText} Removed all online notifications for this guild");
        }

        [Command("dead")]
        [Alias("offline", "shutdown")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        public async Task SetDeadAsync(IMessageChannel channel)
        {
            if (!Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
            {
                await ReplyAsync($"{Res.Str.ErrorText} That channel does not exist on this guild!");
                return;
            }

            var guildData = await TitanbotDatabase.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyDead = channel.Id;
            await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
            await ReplyAsync($"{Res.Str.SuccessText} Set <#{channel.Id}> to recieve a message when I am shutting down!");
        }

        [Command("dead")]
        [Alias("offline", "shutdown")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        public async Task RemoveDeadAsync()
        {
            var guildData = await TitanbotDatabase.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyAlive = null;
            await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
            await ReplyAsync($"{Res.Str.SuccessText} Removed all shut down notifications for this guild");
        }
    }
}
