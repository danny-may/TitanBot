using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Handlers;
using TitanBot2.Extensions;
using System.Collections.Generic;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2
{
    public class TitanBot
    {
        private DiscordSocketClient Client { get; }
        private TitanbotDatabase Database { get; }
        private TimerService TimerService { get; set; }
        private TitanbotDependencies Dependencies { get; }
        public Logger Logger { get; }

        private GuildHandler _GHandle;
        private MessageHandler _MHandle;
        private UserHandler _UHandle;
        private ChannelHandler _CHandle;

        public TitanBot()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,

                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance
            });

            _GHandle = new GuildHandler();
            _MHandle = new MessageHandler();
            _UHandle = new UserHandler();
            _CHandle = new ChannelHandler();

            Logger = new Logger();
            Database = new TitanbotDatabase("database/TitanBot2.db");
            Dependencies = new TitanbotDependencies()
            {
                Client = Client,
                Database = Database,
                Logger = Logger,
                TitanBot = this
            };

            TimerService = new TimerService(Dependencies);
            Dependencies.TimerService = TimerService;

            Client.Log += l => Logger.Log(l);
            Client.LoggedIn += () => LoggedIn?.Invoke() ?? Task.CompletedTask;
            Client.LoggedOut += () => LoggedOut?.Invoke() ?? Task.CompletedTask;
            Client.Connected += () => Connected?.Invoke() ?? Task.CompletedTask;
            Client.Disconnected += e => Disconnected?.Invoke(e) ?? Task.CompletedTask;
            Client.LatencyUpdated += (o, n) => LatencyUpdated?.Invoke(o, n) ?? Task.CompletedTask;
            Client.Ready += () => Ready?.Invoke() ?? Task.CompletedTask;

            Ready += OnReady;

            Database.DefaultExceptionHandler += ex => Logger.Log(ex, "Database");
        }

        public async Task<bool> StartAsync()
        {
            var config = Configuration.Instance;

            if (string.IsNullOrWhiteSpace(config.Token))
                return false;

            Database.Initialise();
            TimerService.Initialise();

            await Client.LoginAsync(TokenType.Bot, config.Token);
            await Client.StartAsync();

            await _CHandle.Install(Dependencies);
            await _GHandle.Install(Dependencies);
            await _MHandle.Install(Dependencies);
            await _UHandle.Install(Dependencies);

            return true;
        }

        public async Task StopAsync(TimeSpan? delay = null, string reason = null)
        {
            var inst = Configuration.Instance;
            inst.ShutdownReason = reason;
            inst.SaveJson();

            IEnumerable<ulong> deadChannels;
            if (delay != null)
            {
                deadChannels = await Database.Guilds.GetDeadChannels(ex => Logger.Log(ex, "StopAsync"));
                await Client.SendToAll(deadChannels, "", embed: Res.Embeds.BuildDeadNotification(Client.CurrentUser, delay, reason));
                await Task.Delay(delay.Value);
            }

            deadChannels = await Database.Guilds.GetDeadChannels(ex => Logger.Log(ex, "StopAsync"));
            await Client.SendToAll(deadChannels, "", embed: Res.Embeds.BuildDeadNotification(Client.CurrentUser, null, reason));

            await Client.LogoutAsync();

            await _CHandle.Uninstall();
            await _GHandle.Uninstall();
            await _MHandle.Uninstall();
            await _UHandle.Uninstall();

            await Database.StopAsync();
            await TimerService.StopAsync();
        }

        private async Task OnReady()
        {
            var aliveChannels = await Database.Guilds.GetAliveChannels(ex => Logger.Log(ex, "StartAsync"));
            await Client.SendToAll(aliveChannels, "", embed: Res.Embeds.BuildAliveNotification(Client.CurrentUser, Configuration.Instance.ShutdownReason));
            var inst = Configuration.Instance;
            inst.ShutdownReason = "Unexpected Crash";
            inst.SaveJson();
        }
        
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
        public event Func<Task> Connected;
        public event Func<Exception, Task> Disconnected;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<Task> Ready;
    }
}
