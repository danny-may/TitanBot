using Discord.WebSocket;
using TitanBot.Logger;

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

            Logger.Log(new LogEntry(TitanBot.Logger.LogSeverity.Info, LogType.Handler, $"Built handler {GetType().Name} | Assembly: {GetType().Assembly}", GetType().Name));
        }
    }
}
