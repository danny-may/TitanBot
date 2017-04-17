using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Handlers
{
    public class GuildHandler
    {
        private DiscordSocketClient _client;
        private TitanBot _bot;

        public void Install(TitanBot b)
        {
            _client = b._client;
            _bot = b;

            _client.GuildAvailable += HandleAvailableAsync;
            _client.GuildUnavailable += HandleUnavailableAsync;
            _client.GuildUpdated += HandleUpdatedAsync;
            _client.JoinedGuild += HandleJoinAsync;
            _client.LeftGuild += HandleLeftAsync;
            _client.RoleCreated += HandleRoleCreateAsync;
            _client.RoleDeleted += HandleRoleDeletedAsync;
            _client.RoleUpdated += HandleRoleUpdateddAsync;

            _bot.Log(new LogEntry(LogType.Handler, "Installed successfully", "GuildHandler"));
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
            _bot = null;

            _bot.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "GuildHandler"));
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
