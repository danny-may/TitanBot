using Discord;
using System.Threading.Tasks;
using TitanBot.Logging;
using TitanBot.Util;

namespace TitanBot.Commands
{
    class Replier : IReplier
    {
        private ILogger Logger { get; }
        private IMessageContext Context { get; }

        public event OnSendEventHandler OnSend;
        public event OnModifyEventHandler OnModify;

        public Replier(IMessageContext context, ILogger logger)
        {
            Logger = logger;
            Context = context;
            OnSend += (s, m) => Task.CompletedTask;
            OnModify += (s, m) => Task.CompletedTask;
        }

        public IReplyContext Reply(IMessageChannel channel)
            => MiscUtil.InlineAction(new ReplyContext(channel, Context), c => c.OnSend += PropogateSend);
        public IModifyContext Modify(IUserMessage message)
            => MiscUtil.InlineAction(new ModifyContext(Context, message), m => m.OnModify += PropogateModify);

        private Task PropogateSend(IReplyContext context, IUserMessage message)
            => OnSend(context, message);
        private Task PropogateModify(IModifyContext context, IUserMessage message)
            => OnModify(context, message);
    }
}
