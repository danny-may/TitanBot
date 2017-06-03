using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Dependencies;
using DiscordBot.Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class BotClient : IDisposable
    {
        public IDiscordClient DiscordClient { get; private set; }
        public ILogger Logger { get; private set; }
        public IBotDb Database { get; private set; }
        public IDependencyManager DependencyManager { get; private set; }

        public BotClient()
            : this(new BotClientConfig()) { }

        public BotClient(string dbLocation, string loggerLocation)
        {
            var config = new BotClientConfig();
            config.Logger = new BotLogger(loggerLocation);
            config.Database = new BotDb(dbLocation, config.Logger);
            LoadFrom(config);
            BuildDependencies();
        }

        public BotClient(BotClientConfig config)
        {
            LoadFrom(config);
            BuildDependencies();
        }

        private void LoadFrom(BotClientConfig config)
        {
            config = config ?? new BotClientConfig();
            DiscordClient = config.DiscordClient ?? new DiscordSocketClient();
            Logger = config.Logger ?? new BotLogger($@".\logs\{DateTime.Now}.log");
            Database = config.Database ?? new BotDb(@".\database\bot.db", Logger);
            DependencyManager = config.DependencyManager ?? new DependencyManager();
        }

        private void BuildDependencies()
        {
            DependencyManager.Add(DiscordClient);
            DependencyManager.Add(Logger);
            DependencyManager.Add(Database);
        }

        public void Dispose()
        {
            Database?.Dispose();
            DiscordClient?.Dispose();
            DependencyManager?.Dispose();
        }
    }
}
