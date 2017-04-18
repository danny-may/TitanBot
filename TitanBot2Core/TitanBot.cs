using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;
using TitanBot2.Handlers;

namespace TitanBot2
{
    public class TitanBot
    {
        public DiscordSocketClient Client { get; }
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

            Client.Log += l => Logger.Log(l);
            Client.LoggedIn += () => LoggedIn?.Invoke() ?? Task.CompletedTask;
            Client.LoggedOut += () => LoggedOut?.Invoke() ?? Task.CompletedTask;
            Client.Connected += () => Connected?.Invoke() ?? Task.CompletedTask;
            Client.Disconnected += e => Disconnected?.Invoke(e) ?? Task.CompletedTask;
            Client.LatencyUpdated += (o, n) => LatencyUpdated?.Invoke(o, n) ?? Task.CompletedTask;
            Client.Ready += () => Ready?.Invoke() ?? Task.CompletedTask;

            TitanbotDatabase.DefaultExceptionHandler += ex => Logger.Log(ex, "Database");
        }

        public async Task<bool> StartAsync()
        {
            var config = Configuration.Instance;

            if (string.IsNullOrWhiteSpace(config.Token))
                return false;

            await Client.LoginAsync(TokenType.Bot, config.Token);
            await Client.StartAsync();

            _CHandle.Install(this);
            _GHandle.Install(this);
            _MHandle.Install(this);
            _UHandle.Install(this);

            return true;
        }

        public async Task StopAsync()
        {
            await Client.LogoutAsync();

            _CHandle.Uninstall();
            _GHandle.Uninstall();
            _MHandle.Uninstall();
            _UHandle.Uninstall();
        }
        
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
        public event Func<Task> Connected;
        public event Func<Exception, Task> Disconnected;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<Task> Ready;
    }
}
