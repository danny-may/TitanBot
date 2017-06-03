using Discord;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase
{
    public class BotClientConfig
    {
        public IBotDb Database { get; set; }
        public IDiscordClient DiscordClient { get; set; }
        public ILogger Logger { get; set; }
        public IDependencyManager DependencyManager { get; set; }
    }
}
