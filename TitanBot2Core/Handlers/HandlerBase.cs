using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Handlers
{
    public abstract class HandlerBase
    {
        protected TitanBot TitanBot { get; private set; }
        protected DiscordSocketClient Client { get; private set; }
        protected TitanbotDatabase Database { get; private set; }
        protected TimerService TimerService { get; private set; }
        protected CachedWebService WebService { get; private set; }

        protected TitanbotDependencies Dependencies { get; private set; }


        public virtual Task Install(TitanbotDependencies args)
        {
            TitanBot = args.TitanBot;
            Client = args.Client;
            Database = args.Database;
            TimerService = args.TimerService;
            WebService = args.WebSerivce;

            Dependencies = args;

            return Task.CompletedTask;
        }

        public virtual Task Uninstall()
        {
            TitanBot = null;
            Client = null;
            Database = null;
            TimerService = null;

            return Task.CompletedTask;
        }
    }
}
