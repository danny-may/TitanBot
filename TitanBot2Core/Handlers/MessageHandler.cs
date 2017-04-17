using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Handlers
{
    public class MessageHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;
        private TitanBot _bot;

        public async void Install(TitanBot b)
        {
            _client = b._client;
            _bot = b;
            _cmds = new CommandService();

            await _cmds.AddModulesAsync(Assembly.GetExecutingAssembly());

            _client.MessageReceived += HandleRecieveAsync;
            _client.MessageDeleted += HandleDeleteAsync;
            _client.MessageUpdated += HandleUpdateAsync;
            _client.ReactionAdded += HandleReactionAddAsync;
            _client.ReactionRemoved += HandleReactionRemoveAsync;
            _client.ReactionsCleared += HandleReactionClearedAsync;

            await _bot.Log(new LogEntry(LogType.Handler, "Installed successfully", "MessageHandler"));
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
            _bot.Log(new LogEntry(LogType.Handler, "Uninstalled successfully", "MessageHandler"));
            _bot = null;
        }

        private async Task HandleRecieveAsync(SocketMessage msg)
        {
            await CheckAndRunCommands(msg);
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

        private async Task CheckAndRunCommands(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot)
                return;

            var context = new TitanbotCmdContext(_bot, msg);
            
            int argPos = 0;
            if (context.CheckCommand(ref argPos))
            {
                var result = await _cmds.ExecuteAsync(context, argPos);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case CommandError.BadArgCount:
                            break;
                        case CommandError.Exception:
                            break;
                        case CommandError.MultipleMatches:
                            break;
                        case CommandError.ObjectNotFound:
                            break;
                        case CommandError.ParseFailed:
                            break;
                        case CommandError.UnknownCommand:
                            await msg.Channel.SendMessageAsync($"{Resources.Str.ErrorText} That is not a recognised command.", ex => _bot.Log(ex, "CheckAndRunCommands"));
                            break;
                        case CommandError.UnmetPrecondition:
                            break;
                    }
            }
        }
    }
}
