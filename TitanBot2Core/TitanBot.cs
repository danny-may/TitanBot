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
        private DiscordSocketClient _client;
        private GuildHandler _GHandle;
        private MessageHandler _MHandle;
        private UserHandler _UHandle;
        private ChannelHandler _CHandle;

        public TitanBot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            });

            _GHandle = new GuildHandler();
            _MHandle = new MessageHandler();
            _UHandle = new UserHandler();
            _CHandle = new ChannelHandler();

            _client.Log += l => Log?.Invoke(l);
            _client.LoggedIn += () => LoggedIn?.Invoke();
            _client.LoggedOut += () => LoggedOut?.Invoke();
            _client.Connected += () => Connected?.Invoke();
            _client.Disconnected += e => Disconnected?.Invoke(e);
            _client.LatencyUpdated += (o, n) => LatencyUpdated?.Invoke(o, n);
            _client.Ready += () => Ready?.Invoke();
        }

        public async Task<bool> StartAsync()
        {
            var config = Configuration.Load();

            if (string.IsNullOrWhiteSpace(config.Token))
                return false;

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            _CHandle.Install(_client);
            _GHandle.Install(_client);
            _MHandle.Install(_client);
            _UHandle.Install(_client);

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

        public event Func<LogMessage, Task> Log;
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
        public event Func<Task> Connected;
        public event Func<Exception, Task> Disconnected;
        public event Func<int, int, Task> LatencyUpdated;
        public event Func<Task> Ready;
    }
}
