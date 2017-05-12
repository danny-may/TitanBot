using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.GuildSpecific;

namespace TitanBot2.DiscordHandlers
{
    public class UserHandler : HandlerBase
    {
        private Dictionary<ulong, GuildSpecificHandler> _guildHandlers = new Dictionary<ulong, GuildSpecificHandler>();

        public override async Task Install(TitanbotDependencies args)
        {
            await base.Install(args);

            _guildHandlers.Add(312312366391885824, new YortKingdom(args));

            Client.UserBanned += HandleBannedAsync;
            Client.UserJoined += HandleJoinAsync;
            Client.UserLeft += HandleLeaveAsync;
            Client.UserUnbanned += HandleUnbanAsync;
            Client.UserUpdated += HandleUpdateAsync;
            Client.GuildMemberUpdated += HandleGUpdateAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Installed successfully", "User"));
        }

        public override async Task Uninstall()
        {
            Client.UserBanned -= HandleBannedAsync;
            Client.UserJoined -= HandleJoinAsync;
            Client.UserLeft -= HandleLeaveAsync;
            Client.UserUnbanned -= HandleUnbanAsync;
            Client.UserUpdated -= HandleUpdateAsync;
            Client.GuildMemberUpdated -= HandleGUpdateAsync;

            await TitanBot.Logger.Log(new LogEntry(LogType.Handler, LogSeverity.Info, "Uninstalled successfully", "User"));
            await base.Uninstall();
        }

        private async Task HandleBannedAsync(SocketUser user, SocketGuild guild)
        {
            if (_guildHandlers.ContainsKey(guild.Id))
                await _guildHandlers[guild.Id].HandleBanned(user, guild);
        }

        private async Task HandleUnbanAsync(SocketUser user, SocketGuild guild)
        {
            if (_guildHandlers.ContainsKey(guild.Id))
                await _guildHandlers[guild.Id].HandleUnban(user, guild);
        }

        private async Task HandleJoinAsync(SocketGuildUser user)
        {
            if (_guildHandlers.ContainsKey(user.Guild.Id))
                await _guildHandlers[user.Guild.Id].HandleJoin(user);
        }

        private async Task HandleLeaveAsync(SocketGuildUser user)
        {
            if (_guildHandlers.ContainsKey(user.Guild.Id))
                await _guildHandlers[user.Guild.Id].HandleLeave(user);
        }

        private async Task HandleUpdateAsync(SocketUser oldUser, SocketUser newUser)
        {

        }

        private async Task HandleGUpdateAsync(SocketGuildUser oldUser, SocketGuildUser newUser)
        {
            if (_guildHandlers.ContainsKey(oldUser.Guild.Id))
                await _guildHandlers[oldUser.Guild.Id].HandleGUpdate(oldUser, newUser);
        }
    }
}
