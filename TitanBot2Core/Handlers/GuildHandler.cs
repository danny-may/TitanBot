using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;

namespace TitanBot2.Handlers
{
    public class GuildHandler
    {
        private DiscordSocketClient _client;
        private TitanBot _bot;

        public void Install(TitanBot b)
        {
            _client = b.Client;
            _bot = b;

            _client.GuildAvailable += HandleAvailableAsync;
            _client.GuildUnavailable += HandleUnavailableAsync;
            _client.GuildUpdated += HandleUpdatedAsync;
            _client.JoinedGuild += HandleJoinAsync;
            _client.LeftGuild += HandleLeftAsync;
            _client.RoleCreated += HandleRoleCreateAsync;
            _client.RoleDeleted += HandleRoleDeletedAsync;
            _client.RoleUpdated += HandleRoleUpdateddAsync;

            b.Logger.Log(new LogEntry(LogType.Handler, "Installed successfully", "GuildHandler"));
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

            _bot.Logger.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "GuildHandler"));

            _client = null;
            _bot = null;

        }

        private async Task HandleAvailableAsync(SocketGuild guild)
        {
            await TitanbotDatabase.Guilds.EnsureExists(guild.Id);
        }

        private async Task HandleUnavailableAsync(SocketGuild guild)
        {

        }

        private async Task HandleUpdatedAsync(SocketGuild before, SocketGuild after)
        {

        }

        private async Task HandleJoinAsync(SocketGuild guild)
        {
            await TitanbotDatabase.Guilds.EnsureExists(guild.Id);
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
