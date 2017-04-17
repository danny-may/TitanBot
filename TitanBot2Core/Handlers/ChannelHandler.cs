using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Handlers
{
    public class ChannelHandler
    {
        private DiscordSocketClient _client;
        private TitanBot _bot;

        public void Install(TitanBot b)
        {
            _client = b._client;
            _bot = b;

            _client.ChannelCreated += HandleCreatedAsync;
            _client.ChannelDestroyed += HandleDestroyedAsync;
            _client.ChannelUpdated += HandleUpdatedAsync;
            _client.RecipientAdded += HandleRecipientAddedAsync;
            _client.RecipientRemoved += HandleRecipientRemovedAsync;

            _bot.Log(new LogEntry(LogType.Handler, "Installed successfully", "ChannelHandler"));
        }

        public void Uninstall()
        {
            _client.ChannelCreated -= HandleCreatedAsync;
            _client.ChannelDestroyed -= HandleDestroyedAsync;
            _client.ChannelUpdated -= HandleUpdatedAsync;

            _client.RecipientAdded -= HandleRecipientAddedAsync;
            _client.RecipientRemoved -= HandleRecipientRemovedAsync;

            _client = null;
            _bot = null;
            
            _bot.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "ChannelHandler"));
        }

        private async Task HandleCreatedAsync(SocketChannel channel)
        {

        }

        private async Task HandleDestroyedAsync(SocketChannel channel)
        {

        }

        private async Task HandleUpdatedAsync(SocketChannel oldChannel, SocketChannel newChannel)
        {

        }

        private async Task HandleRecipientAddedAsync(SocketGroupUser user)
        {

        }

        private async Task HandleRecipientRemovedAsync(SocketGroupUser user)
        {

        }
    }
}
