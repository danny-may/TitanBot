using Discord.WebSocket;
using System.Threading.Tasks;

namespace TitanBot2.Handlers
{
    public class GuildHandler
    {
        private DiscordSocketClient _client;

        public void Install(DiscordSocketClient c)
        {
            _client = c;

            _client.GuildAvailable += HandleAvailableAsync;
            _client.GuildUnavailable += HandleUnavailableAsync;
            _client.GuildUpdated += HandleUpdatedAsync;
            _client.JoinedGuild += HandleJoinAsync;
            _client.LeftGuild += HandleLeftAsync;
            _client.RoleCreated += HandleRoleCreateAsync;
            _client.RoleDeleted += HandleRoleDeletedAsync;
            _client.RoleUpdated += HandleRoleUpdateddAsync;
        }

        public void Uninstall()
        {
            _client.GuildAvailable -= HandleAvailableAsync;
            _client.GuildUnavailable -= HandleUnavailableAsync;
            _client.GuildUpdated -= HandleUpdatedAsync;
            _client.JoinedGuild -= HandleJoinAsync;
            _client.LeftGuild -= HandleLeftAsync;
            _client.RoleCreated -= HandleRoleCreateAsync;
            _client.RoleDeleted -= HandleRoleDeletedAsync;
            _client.RoleUpdated -= HandleRoleUpdateddAsync;

            _client = null;
        }

        private async Task HandleAvailableAsync(SocketGuild guild)
        {

        }

        private async Task HandleUnavailableAsync(SocketGuild guild)
        {

        }

        private async Task HandleUpdatedAsync(SocketGuild before, SocketGuild after)
        {

        }

        private async Task HandleJoinAsync(SocketGuild guild)
        {

        }

        private async Task HandleLeftAsync(SocketGuild guild)
        {

        }

        private async Task HandleRoleCreateAsync(SocketRole role)
        {

        }

        private async Task HandleRoleDeletedAsync(SocketRole role)
        {

        }

        private async Task HandleRoleUpdateddAsync(SocketRole oldRole, SocketRole newRole)
        {

        }
    }
}
