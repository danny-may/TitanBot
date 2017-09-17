using Discord;

namespace TitanBot.Core.Models.Contexts
{
    public interface IMessageContext : IGuildContext, IChannelContext, IUserContext
    {
        IUserMessage Message { get; }
    }
}