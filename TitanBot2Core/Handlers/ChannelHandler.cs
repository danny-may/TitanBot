using Discord.WebSocket;
using System.Threading.Tasks;

namespace TitanBot2.Handlers
{
    public class ChannelHandler
    {
        private DiscordSocketClient _client;

        public void Install(DiscordSocketClient c)
        {
            _client = c;

            _client.ChannelCreated += HandleCreatedAsync;
            _client.ChannelDestroyed += HandleDestroyedAsync;
            _client.ChannelUpdated += HandleUpdatedAsync;
            _client.RecipientAdded += HandleRecipientAddedAsync;
            _client.RecipientRemoved += HandleRecipientRemovedAsync;
        }

        public void Uninstall()
        {
            _client.ChannelCreated -= HandleCreatedAsync;
            _client.ChannelDestroyed -= HandleDestroyedAsync;
            _client.ChannelUpdated -= HandleUpdatedAsync;

            _client.RecipientAdded -= HandleRecipientAddedAsync;
            _client.RecipientRemoved -= HandleRecipientRemovedAsync;

            _client = null;
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
