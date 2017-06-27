using Discord.WebSocket;
using TitanBot.Logging;

namespace TitanBot.DiscordHandlers
{
    public abstract class DiscordHandlerBase
    {
        protected DiscordSocketClient Client { get; }
        protected ILogger Logger { get; }

        public DiscordHandlerBase(DiscordSocketClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;

            Logger.Log(new LogEntry(TitanBot.Logging.LogSeverity.Info, LogType.Handler, $"Built handler {GetType().Name} | Assembly: {GetType().Assembly}", GetType().Name));
        }
    }
}
