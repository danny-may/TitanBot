using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Logging;
using TitanBot.Storage;

namespace TitanBot.DiscordHandlers
{
    class CoreHandler : DiscordHandlerBase
    {
        ICommandService CommandService { get; }
        IDatabase Database { get; }
        public CoreHandler(DiscordSocketClient client, ILogger logger, ICommandService cmdService, IDatabase database) : base(client, logger)
        {
            CommandService = cmdService;
            Database = database;
            client.MessageReceived += MessageRecievedAsync;
        }

        private Task MessageRecievedAsync(SocketMessage msg)
        {
            if (msg is IUserMessage message)
                Task.Run(() => CommandService.ParseAndExecute(message));
            return Task.CompletedTask;
        }
    }
}
