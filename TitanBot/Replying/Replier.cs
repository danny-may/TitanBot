using Discord;
using TitanBot.Dependencies;
using TitanBot.Logging;

namespace TitanBot.Replying
{
    class Replier : IReplier
    {
        private ILogger Logger { get; }
        private IDependencyFactory Factory { get; }

        public Replier(ILogger logger, IDependencyFactory factory)
        {
            Logger = logger;
            Factory = factory;
        }

        public IReplyContext Reply(IMessageChannel channel, IUser user)
            => new ReplyContext(channel, user, Factory, Logger);
        public IModifyContext Modify(IUserMessage message, IUser user)
            => new ModifyContext(message, user, Factory, Logger);
    }
}
