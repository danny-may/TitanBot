using Discord;
using Discord.WebSocket;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;
using TitanBotBase.Logger;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.SchedulerService;

namespace TitanBotBase
{
    public class BotClient : IDisposable
    {
        public IDiscordClient DiscordClient { get; private set; }
        public ILogger Logger { get; private set; }
        public IDatabase Database { get; private set; }
        public IDependencyManager DependencyManager { get; private set; }
        public IScheduler Scheduler { get; private set; }

        public BotClient()
            : this(new BotClientConfig()) { }

        public BotClient(string dbLocation, string loggerLocation)
        {
            var config = new BotClientConfig();
            config.Logger = new TitanBotLogger(loggerLocation);
            config.Database = new TitanBotDb(dbLocation, config.Logger);
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
            Logger = config.Logger ?? new TitanBotLogger($@".\logs\{DateTime.Now}.log");
            Database = config.Database ?? new TitanBotDb(@".\database\bot.db", Logger);
            DependencyManager = config.DependencyManager ?? new DependencyManager();
            Scheduler = config.Scheduler ?? new Scheduler(DependencyManager, Database, Logger);
        }

        private void BuildDependencies()
        {
            DependencyManager.Add(DiscordClient);
            DependencyManager.Add(Logger);
            DependencyManager.Add(Database);
            DependencyManager.Add(Scheduler);
        }

        public void Dispose()
        {
            Database?.Dispose();
            DiscordClient?.Dispose();
            DependencyManager?.Dispose();
        }
    }
}
