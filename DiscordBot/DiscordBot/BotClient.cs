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
        public DiscordSocketClient DiscordClient { get; }
        public IBotDb Database { get; }


        public BotClient(string dbLocation)
        {
            Database = new BotDb(dbLocation);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
