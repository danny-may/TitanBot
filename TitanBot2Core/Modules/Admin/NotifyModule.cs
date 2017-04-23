using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.Admin
{
    public partial class AdminModule
    {
        [Group("Notify")]
        [RequireContext(ContextType.Guild)]
        [RequireCustomPermission(8)]
        [Summary("Subscribes or unsubscribes a channel from bot notifications")]
        public class NotifyModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Sets a channel for the given notification type to appear in. Omitting a channel will remove it.\nValid types are:\nalive/online\ndead/offline/shutdown)")]
            [Priority(-10)]
            public async Task ListTypesAsync(NotificationType notificationType, IMessageChannel channel = null)
            {
                switch (notificationType)
                {
                    case NotificationType.alive:
                    case NotificationType.online:
                        await SetAliveAsync(channel);
                        break;
                    case NotificationType.dead:
                    case NotificationType.offline:
                        await SetDeadAsync(channel);
                        break;
                }
            }

            public enum NotificationType
            {
                alive,
                online,
                dead,
                offline
            }

            public async Task SetAliveAsync(IMessageChannel channel)
            {
                if (channel != null && !Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
                {
                    await ReplyAsync($"{Res.Str.ErrorText} That channel does not exist on this guild!");
                    return;
                }

                var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
                guildData.NotifyAlive = channel?.Id;
                await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
                if (channel == null)
                    await ReplyAsync($"{Res.Str.SuccessText} this guild will no longer recieve notification when I come onine!");
                else
                    await ReplyAsync($"{Res.Str.SuccessText} Set <#{channel.Id}> to recieve a message when I come online!");

            }

            public async Task SetDeadAsync(IMessageChannel channel)
            {
                if (channel != null && !Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
                {
                    await ReplyAsync($"{Res.Str.ErrorText} That channel does not exist on this guild!");
                    return;
                }

                var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
                guildData.NotifyDead = channel.Id;
                await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.TitanBot.Logger.Log(ex, "NotifyCmd"));
                if (channel == null)
                    await ReplyAsync($"{Res.Str.SuccessText} this guild will no longer recieve notification when I shutdown!");
                else
                    await ReplyAsync($"{Res.Str.SuccessText} Set <#{channel.Id}> to recieve a message when I am shutting down!");
            }
        }
    }
}
