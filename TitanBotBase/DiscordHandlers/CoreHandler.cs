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
            client.GuildAvailable += GuildAvailableAsync;
        }

        private Task GuildAvailableAsync(SocketGuild arg)
        {
            Database.QueryAsync(conn => conn.GetTable<Guild>().Ensure(new Guild
            {
                Id = arg.Id
            }));
            return Task.CompletedTask;
        }

        private Task MessageRecievedAsync(SocketMessage arg)
        {
            if (arg is IUserMessage message)
                Task.Run(() => CommandService.ParseAndExecute(message));
            return Task.CompletedTask;
        }
    }
}
