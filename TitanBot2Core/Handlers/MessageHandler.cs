using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBot2.Handlers
{
    public class MessageHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async void Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleRecieveAsync;
            _client.MessageDeleted += HandleDeleteAsync;
            _client.MessageUpdated += HandleUpdateAsync;
            _client.ReactionAdded += HandleReactionAddAsync;
            _client.ReactionRemoved += HandleReactionRemoveAsync;
            _client.ReactionsCleared += HandleReactionClearedAsync;
        }

        public void Uninstall()
        {
            _client.MessageReceived -= HandleRecieveAsync;
            _client.MessageDeleted -= HandleDeleteAsync;
            _client.MessageUpdated -= HandleUpdateAsync;
            _client.ReactionAdded -= HandleReactionAddAsync;
            _client.ReactionRemoved -= HandleReactionRemoveAsync;
            _client.ReactionsCleared -= HandleReactionClearedAsync;

            _cmds = null;
            _client = null;
        }

        private async Task HandleRecieveAsync(SocketMessage msg)
        {

        }

        private async Task HandleDeleteAsync(Cacheable<IMessage, ulong> cachedMsg, ISocketMessageChannel channel)
        {

        }

        private async Task HandleUpdateAsync(Cacheable<IMessage, ulong> cachedMsg, SocketMessage msg, ISocketMessageChannel channel)
        {

        }

        private async Task HandleReactionAddAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel, SocketReaction reaction)
        {

        }

        private async Task HandleReactionRemoveAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel, SocketReaction reaction)
        {

        }

        private async Task HandleReactionClearedAsync(Cacheable<IUserMessage, ulong> cachedMsg, ISocketMessageChannel channel)
        {

        }
    }
}
