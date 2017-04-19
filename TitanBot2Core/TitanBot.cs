using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database;
using TitanBot2.Handlers;
using TitanBot2.Extensions;
using System.Collections.Generic;

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

            Ready += OnReady;

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

        public async Task StopAsync(TimeSpan? delay = null, string reason = null)
        {
            var inst = Configuration.Instance;
            inst.ShutdownReason = reason;
            inst.SaveJson();

            IEnumerable<ulong> deadChannels;
            if (delay != null)
            {
                deadChannels = await TitanbotDatabase.Guilds.GetDeadChannels(ex => Logger.Log(ex, "StopAsync"));
                await Client.SendToAll(deadChannels, "", embed: Res.Embeds.BuildDeadNotification(delay, reason));
                await Task.Delay(delay.Value);
            }

            deadChannels = await TitanbotDatabase.Guilds.GetDeadChannels(ex => Logger.Log(ex, "StopAsync"));
            await Client.SendToAll(deadChannels, "", embed: Res.Embeds.BuildDeadNotification(null, reason));

            await Client.LogoutAsync();

            _CHandle.Uninstall();
            _GHandle.Uninstall();
            _MHandle.Uninstall();
            _UHandle.Uninstall();
        }

        private async Task OnReady()
        {
            var aliveChannels = await TitanbotDatabase.Guilds.GetAliveChannels(ex => Logger.Log(ex, "StartAsync"));
            await Client.SendToAll(aliveChannels, "", embed: Res.Embeds.BuildAliveNotification(Configuration.Instance.ShutdownReason));
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
