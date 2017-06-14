using Discord.WebSocket;
using TitanBotBase.Logger;

namespace TitanBotBase.DiscordHandlers
{
    public abstract class DiscordHandlerBase
    {
        protected DiscordSocketClient Client { get; }
        protected ILogger Logger { get; }

        public DiscordHandlerBase(DiscordSocketClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;

            Logger.Log(new LogEntry(TitanBotBase.Logger.LogSeverity.Info, LogType.Handler, $"Built handler {GetType().Name} | Assembly: {GetType().Assembly}", GetType().Name));
        }
    }
}
