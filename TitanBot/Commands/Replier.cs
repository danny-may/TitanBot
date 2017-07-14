using Discord;
using TitanBot.Formatting;
using TitanBot.Logging;

namespace TitanBot.Commands
{
    public class Replier : IReplier
    {
        private ILogger Logger { get; }
        private ITextResourceCollection TextResource { get; }
        private ValueFormatter Formatter { get; }

        public Replier(ILogger logger, ITextResourceCollection textResource, ValueFormatter formatter)
        {
            Logger = logger;
            TextResource = textResource;
            Formatter = formatter;
        }

        public IReplyContext Reply(IMessageChannel channel, IUser user)
            => new ReplyContext(channel, user, TextResource, Formatter);
    }
}
