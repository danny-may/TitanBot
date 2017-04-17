using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Handlers;

namespace TitanBot2
{
    public class TitanBot
    {
        public DiscordSocketClient _client;
        private GuildHandler _GHandle;
        private MessageHandler _MHandle;
        private UserHandler _UHandle;
        private ChannelHandler _CHandle;

        public TitanBot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,

                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance
            });

            _GHandle = new GuildHandler();
            _MHandle = new MessageHandler();
            _UHandle = new UserHandler();
            _CHandle = new ChannelHandler();

            _client.Log += l => Log(LogEntry.FromClientLog(l));
            _client.LoggedIn += () => LoggedIn?.Invoke() ?? Task.CompletedTask;
            _client.LoggedOut += () => LoggedOut?.Invoke() ?? Task.CompletedTask;
            _client.Connected += () => Connected?.Invoke() ?? Task.CompletedTask;
            _client.Disconnected += e => Disconnected?.Invoke(e) ?? Task.CompletedTask;
            _client.LatencyUpdated += (o, n) => LatencyUpdated?.Invoke(o, n) ?? Task.CompletedTask;
            _client.Ready += () => Ready?.Invoke() ?? Task.CompletedTask;
        }

        public async Task<bool> StartAsync()
        {
            var config = Configuration.Instance;

            if (string.IsNullOrWhiteSpace(config.Token))
                return false;

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            _CHandle.Install(this);
            _GHandle.Install(this);
            _MHandle.Install(this);
            _UHandle.Install(this);

            return true;
        }

        public async Task StopAsync()
        {
            await _client.LogoutAsync();

            _CHandle.Uninstall();
            _GHandle.Uninstall();
            _MHandle.Uninstall();
            _UHandle.Uninstall();
        }

        internal Task Log(LogEntry entry)
        {
            return (LogEntryAdded?.Invoke(entry) ?? Task.CompletedTask);
        }
        internal Task Log(Exception ex, string source)
            => Log(LogEntry.FromException(ex, source));

        public event Func<LogEntry, Task> LogEntryAdded;
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
        public event Func<Task> Connected;
        public event Func<Exception, Task> Disconnected;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<Task> Ready;
    }
}
