using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Core.Command.Interfaces;
using Titanbot.Core.Command.Splitters;
using Titanbot.Core.Config;
using Titanbot.Core.Extensions;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.Core.Command
{
    public class CommandService : ICommandService
    {
        #region Fields

        private readonly ILoggerService _logger;
        private readonly BotConfig _config;
        private readonly DiscordSocketClient _client;
        private readonly IMessageSplitter _msgSplitter;

        #endregion Fields

        #region Constructors

        public CommandService(DiscordSocketClient client, ILoggerService logger, BotConfig config, IMessageSplitter messageSplitter = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _msgSplitter = messageSplitter ?? new MessageSplitter(_client, _config);
            Log += m => _logger.LogAsync(m);

            _client.MessageReceived += HandleMessage;
        }

        #endregion Constructors

        #region Methods

        private void Foo()
        {
        }

        #endregion Methods

        #region ICommandService

        public event Func<LogMessage, Task> Log;

        public async Task HandleMessage(SocketMessage message)
        {
            if (message.Author.IsBot || !(message is SocketUserMessage userMessage))
                return;

            if (message.Channel.Id != 312497138137563136)
                return;

            var escapedText = Format.Sanitize(userMessage.Content);

            var builder = new EmbedBuilder
            {
                Title = "Message found",
                Description = $"Content:\n```{escapedText}```"
            };

            if (!_msgSplitter.TryParseMessage(userMessage, out var prefix, out var cmdName, out var args, out var flags))
                builder.AddField("Message is command", "False");
            else
                builder.AddField("Message is command", "True")
                       .AddField("Prefix", prefix)
                       .AddField("Command Name", cmdName)
                       .AddField("Arguments", $"`{string.Join("`, `", args?.Select(a => Format.Sanitize(a)))}`")
                       .AddField("Flags", $"`{string.Join("`, `", flags?.Select(f => Format.Sanitize(f.ToString())))}`");

            await userMessage.Channel.SendMessageAsync("", embed: builder.Build());
        }

        public void Install<TCommand>() where TCommand : Command
        {
            throw new NotImplementedException();
        }

        public void Install(Type commandType)
        {
            throw new NotImplementedException();
        }

        public void Install(Type[] commands)
        {
            throw new NotImplementedException();
        }

        public void Install(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        #endregion ICommandService
    }
}