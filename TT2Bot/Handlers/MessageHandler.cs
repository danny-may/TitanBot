using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.DiscordHandlers;
using TitanBotBase.Logger;

namespace TT2Bot.Handlers
{
    class MessageHandler : DiscordHandlerBase
    {
        private readonly ICommandService _commandService;

        public MessageHandler(DiscordSocketClient client, ILogger logger, ICommandService commandService) : base(client, logger)
        {
            _commandService = commandService;

            Client.MessageReceived += MessageRecieved;
        }

        private async Task MessageRecieved(SocketMessage msg)
        {
            await Logger.LogAsync(LogSeverity.Verbose, LogType.Message, msg.ToString(), "MessageHandler");
        }
    }
}
