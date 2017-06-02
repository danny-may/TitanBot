using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class BotClient : IDisposable
    {
        public IDiscordClient DiscordClient { get; }
        public IBotDb Database { get; }


        public BotClient(string dbLocation)
        {
            Database = new BotDb(dbLocation);
            DiscordClient = new DiscordSocketClient();
        }

        public void Dispose()
        {
            Database?.Dispose();
            DiscordClient?.Dispose();
        }
    }
}
