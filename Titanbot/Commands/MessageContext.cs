using Discord.WebSocket;
using Titanbot.Commands.Interfaces;
using Titanbot.Commands.Models;

namespace Titanbot.Commands
{
    public class MessageContext
    {
        #region Fields

        public SocketUserMessage Message { get; }
        public SocketUser User => Message.Author;
        public ISocketMessageChannel Channel => Message.Channel;
        public SocketGuild Guild => (Channel as SocketGuildChannel)?.Guild;
        public bool IsCommand { get; }
        public string Prefix { get; }
        public string CommandName { get; }
        public string RawArguments { get; }
        public string[] Arguments { get; }
        public FlagValue[] Flags { get; }

        #endregion Fields

        #region Constructors

        public MessageContext(SocketUserMessage message,
                              IMessageSplitter splitter)
        {
            Message = message;
            IsCommand = splitter.TryParseMessage(Message, out var prefix,
                                                          out var cmdName,
                                                          out var rawArg,
                                                          out var args,
                                                          out var flags);
            if (IsCommand)
            {
                Prefix = prefix;
                CommandName = cmdName;
                RawArguments = rawArg;
                Arguments = args;
                Flags = flags;
            }
        }

        #endregion Constructors

        #region Methods

        public void Void()
        {
        }

        #endregion Methods
    }
}