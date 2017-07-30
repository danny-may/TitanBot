using Discord;

namespace TitanBot.Commands
{
    public interface IReplier
    {
        event OnSendEventHandler OnSend;
        event OnModifyEventHandler OnModify;

        IReplyContext Reply(IMessageChannel channel);
        IModifyContext Modify(IUserMessage message);
    }
}
