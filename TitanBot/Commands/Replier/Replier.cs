using Discord;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Logging;
using TitanBot.Util;

namespace TitanBot.Commands
{
    class Replier : IReplier
    {
        private ILogger Logger { get; }
        private ICommandContext Context { get; }

        public event OnSendEventHandler OnSend;

        public Replier(ICommandContext context, ILogger logger)
        {
            Logger = logger;
            Context = context;
            OnSend += (s, m) => Task.CompletedTask;
        }

        public IReplyContext Reply(IMessageChannel channel)
            => MiscUtil.InlineAction(new ReplyContext(channel, Context), c => c.OnSend += PropogateSend);

        private Task PropogateSend(IReplyContext context, IUserMessage message)
            => OnSend(context, message);
    }
}
