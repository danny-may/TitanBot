using Discord;
using DiscordBot.Database;
using DiscordBot.Dependencies;
using DiscordBot.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class BotClientConfig
    {
        public IBotDb Database { get; set; }
        public IDiscordClient DiscordClient { get; set; }
        public ILogger Logger { get; set; }
        public IDependencyManager DependencyManager { get; set; }
    }
}
