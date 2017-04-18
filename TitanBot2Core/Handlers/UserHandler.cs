using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Handlers
{
    public class UserHandler
    {
        private DiscordSocketClient _client;
        private TitanBot _bot;

        public void Install(TitanBot b)
        {
            _client = b.Client;
            _bot = b;

            _client.UserBanned += HandleBannedAsync;
            _client.UserJoined += HandleJoinAsync;
            _client.UserLeft += HandleLeaveAsync;
            _client.UserUnbanned += HandleUnbanAsync;
            _client.UserUpdated += HandleUpdateAsync;
            _client.GuildMemberUpdated += HandleGUpdateAsync;

            b.Logger.Log(new LogEntry(LogType.Handler, "Installed successfully", "UserHandler"));
        }

        public void Uninstall()
        {
            _client.UserBanned -= HandleBannedAsync;
            _client.UserJoined -= HandleJoinAsync;
            _client.UserLeft -= HandleLeaveAsync;
            _client.UserUnbanned -= HandleUnbanAsync;
            _client.UserUpdated -= HandleUpdateAsync;
            _client.GuildMemberUpdated -= HandleGUpdateAsync;

            _bot.Logger.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "UserHandler"));

            _client = null;
            _bot = null;

            
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
