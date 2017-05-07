using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.DiscordHandlers
{
    public class UserHandler : HandlerBase
    {

        public override async Task Install(TitanbotDependencies args)
        {
            await base.Install(args);

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

        }

        private async Task HandleUnbanAsync(SocketUser user, SocketGuild guild)
        {

        }

        private async Task HandleJoinAsync(SocketGuildUser user)
        {

        }

        private async Task HandleLeaveAsync(SocketGuildUser user)
        {

        }

        private async Task HandleUpdateAsync(SocketUser oldUser, SocketUser newUser)
        {

        }

        private async Task HandleGUpdateAsync(SocketGuildUser oldUser, SocketGuildUser newUser)
        {

        }
    }
}
