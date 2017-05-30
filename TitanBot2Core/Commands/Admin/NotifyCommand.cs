using Discord;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Commands.Admin
{
    [Description("Used to enable/disable notifications of various bot events")]
    [Notes("Valid notifications are:\nalive (online)\ndead (offline)")]
    [RequireContext(ContextType.Guild)]
    [DefaultPermission(8)]
    class NotifyCommand : Command
    {
        [Call]
        [Usage("Sets which channel each notification should appear in.")]
        async Task SwitchTypesAsync(NotificationType notificationType, IMessageChannel channel = null)
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

        enum NotificationType
        {
            alive,
            online,
            dead,
            offline
        }

        async Task SetAliveAsync(IMessageChannel channel)
        {
            if (channel != null && !Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
            {
                await ReplyAsync("That channel does not exist on this guild!", ReplyType.Error);
                return;
            }

            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyAlive = channel?.Id;
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.BotClient.Logger.Log(ex, "NotifyCmd"));
            if (channel == null)
                await ReplyAsync("This guild will no longer recieve notification when I come onine!", ReplyType.Success);
            else
                await ReplyAsync($"Set <#{channel.Id}> to recieve a message when I come online!", ReplyType.Success);

        }

        async Task SetDeadAsync(IMessageChannel channel)
        {
            if (channel != null && !Context.Guild.Channels.Select(c => c.Id).Contains(channel.Id))
            {
                await ReplyAsync("That channel does not exist on this guild!", ReplyType.Error);
                return;
            }

            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.NotifyDead = channel?.Id;
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData), ex => Context.BotClient.Logger.Log(ex, "NotifyCmd"));
            if (channel == null)
                await ReplyAsync("This guild will no longer recieve notification when I shutdown!", ReplyType.Success);
            else
                await ReplyAsync($"Set <#{channel.Id}> to recieve a message when I am shutting down!", ReplyType.Success);
        }
    }
}
