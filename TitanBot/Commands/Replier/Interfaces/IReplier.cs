using Discord;
using System;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public interface IReplier
    {
        event OnSendEventHandler OnSend;

        IReplyContext Reply(IMessageChannel channel);
    }
}
