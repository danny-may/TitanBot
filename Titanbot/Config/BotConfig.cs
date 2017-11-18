using Discord;
using Discord.WebSocket;

namespace Titanbot.Config
{
    public class BotConfig
    {
        public string Token { get; set; } = "";
        public string DefaultPrefix { get; set; } = "";
        public string GatewayHost { get; set; } = null;
        public int ConnectionTimeout { get; set; } = 30000;
        public int? ShardId { get; set; } = null;
        public int? TotalShards { get; set; } = null;
        public int MessageCacheSize { get; set; } = 0;
        public int LargeThreshold { get; set; } = 250;
        public bool AlwaysDownloadUsers { get; set; } = false;
        public int HandlerTimeout { get; set; } = 3000;
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.AlwaysRetry;
        public LogSeverity LogLevel { get; set; } = LogSeverity.Info;

        public DiscordSocketConfig SocketConfig()
            => new DiscordSocketConfig
            {
                GatewayHost = GatewayHost,
                ConnectionTimeout = ConnectionTimeout,
                ShardId = ShardId,
                TotalShards = TotalShards,
                MessageCacheSize = MessageCacheSize,
                LargeThreshold = LargeThreshold,
                AlwaysDownloadUsers = AlwaysDownloadUsers,
                HandlerTimeout = HandlerTimeout,
                DefaultRetryMode = DefaultRetryMode,
                LogLevel = LogLevel
            };
    }
}