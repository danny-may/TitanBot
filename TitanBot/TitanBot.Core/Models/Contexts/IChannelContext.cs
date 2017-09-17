using Discord;

namespace TitanBot.Core.Models.Contexts
{
    public interface IChannelContext
    {
        IMessageChannel Channel { get; }
    }
}