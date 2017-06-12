using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands
{
    public interface IReplier
    {
        Task<IUserMessage> ReplyAsync(IMessageChannel channel, IUser user, string message, ReplyType replyType = ReplyType.None, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
        IUserMessage Reply(IMessageChannel channel, IUser user, string message, ReplyType replyType = ReplyType.None, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
    }
}
