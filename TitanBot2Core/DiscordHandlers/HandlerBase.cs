using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.DiscordHandlers
{
    public abstract class HandlerBase
    {
        protected BotClient BotClient { get; private set; }
        protected DiscordSocketClient Client { get; private set; }
        protected BotDatabase Database { get; private set; }
        protected TimerService TimerService { get; private set; }
        protected CachedWebClient WebService { get; private set; }

        protected BotDependencies Dependencies { get; private set; }


        public virtual Task Install(BotDependencies args)
        {
            BotClient = args.BotClient;
            Client = args.Client;
            Database = args.Database;
            TimerService = args.TimerService;
            WebService = args.WebClient;

            Dependencies = args;

            return Task.CompletedTask;
        }

        public virtual Task Uninstall()
        {
            BotClient = null;
            Client = null;
            Database = null;
            TimerService = null;

            return Task.CompletedTask;
        }
    }
}
