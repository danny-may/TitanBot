using Discord;
using TitanBot.Core.Models.Contexts;

namespace TitanBot.Models.Contexts
{
    public class ChannelContext : IChannelContext
    {
        public ChannelContext(IMessageChannel channel)
        {
            Channel = channel;
        }

        #region IChannelContext

        public IMessageChannel Channel { get; }

        #endregion IChannelContext
    }
}