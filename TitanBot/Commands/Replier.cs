using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Logging;
using TitanBot.TextResource;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public class Replier : IReplier
    {
        private ILogger Logger { get; }
        private ITextResourceCollection TextResource { get; }

        public Replier(ILogger logger, ITextResourceCollection textResource)
        {
            Logger = logger;
            TextResource = textResource;
        }

        public IReplyContext Reply(IMessageChannel channel, IUser user)
            => new ReplyContext(channel, user, TextResource);
    }
}
