using Discord.WebSocket;
using System;
using Titansmasher.Services.Configuration.Interfaces;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.Command
{
    public abstract class Command : IDisposable
    {
        #region Fields

        #region Protected

        protected internal CommandContext Context { get; internal set; }
        protected internal IDatabaseService Database { get; internal set; }
        protected internal IConfigService ConfigService { get; internal set; }
        protected internal ILoggerService Logger { get; internal set; }
        protected internal DiscordSocketClient Client { get; internal set; }

        #endregion Protected

        #endregion Fields

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable
    }
}