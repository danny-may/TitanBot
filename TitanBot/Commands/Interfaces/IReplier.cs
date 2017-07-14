using Discord;
using System;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public interface IReplier
    {
        IReplyContext Reply(IMessageChannel channel, IUser user);
    }
}
