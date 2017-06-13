using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using TitanBotBase.Logger;
using TitanBotBase.Commands;
using Discord;
using TitanBotBase.Database;
using TitanBotBase.Database.Tables;
using TitanBotBase.Settings;

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
