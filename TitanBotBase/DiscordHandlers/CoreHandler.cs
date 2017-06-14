using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Database;
using TitanBotBase.Logger;

namespace TitanBotBase.DiscordHandlers
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

        private Task MessageRecievedAsync(SocketMessage arg)
        {
            if (arg is IUserMessage message)
                Task.Run(() => CommandService.ParseAndExecute(message));
            return Task.CompletedTask;
        }
    }
}
