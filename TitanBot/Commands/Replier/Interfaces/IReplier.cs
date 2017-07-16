using Discord;

namespace TitanBot.Commands
{
    public interface IReplier
    {
        event OnSendEventHandler OnSend;

        IReplyContext Reply(IMessageChannel channel);
    }
}
