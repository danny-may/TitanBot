using Discord.WebSocket;
using System.Threading.Tasks;

namespace TitanBot2.Handlers
{
    public class UserHandler
    {
        private DiscordSocketClient _client;

        public void Install(DiscordSocketClient c)
        {
            _client = c;

            _client.UserBanned += HandleBannedAsync;
            _client.UserJoined += HandleJoinAsync;
            _client.UserLeft += HandleLeaveAsync;
            _client.UserUnbanned += HandleUnbanAsync;
            _client.UserUpdated += HandleUpdateAsync;
            _client.GuildMemberUpdated += HandleGUpdateAsync;
        }

        public void Uninstall()
        {
            _client.UserBanned -= HandleBannedAsync;
            _client.UserJoined -= HandleJoinAsync;
            _client.UserLeft -= HandleLeaveAsync;
            _client.UserUnbanned -= HandleUnbanAsync;
            _client.UserUpdated -= HandleUpdateAsync;
            _client.GuildMemberUpdated -= HandleGUpdateAsync;

            _client = null;
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
