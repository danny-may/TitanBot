using Discord;

namespace TitanBot.Replying
{
    public interface IReplier
    {
        IReplyContext Reply(IMessageChannel channel, IUser user);
        IModifyContext Modify(IUserMessage message, IUser user);
    }
}
