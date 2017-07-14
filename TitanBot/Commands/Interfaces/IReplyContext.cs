using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public delegate Task OnSendEventHandler(IReplyContext context, IUserMessage message);
    public interface IReplyContext
    {
        event OnSendEventHandler OnSend;

        IReplyContext WithMessage(string message);
        IReplyContext WithMessage(string message, ReplyType replyType);
        IReplyContext WithMessage(string message, params object[] values);
        IReplyContext WithMessage(string message, ReplyType replyType, params object[] values);
        IReplyContext WithEmbed(Embed embed);
        IReplyContext WithHandler(Func<Exception, Task> handler);
        IReplyContext WithTTS(bool tts);
        IReplyContext WithRequestOptions(RequestOptions options);

        Task<IUserMessage> SendAsync();
        IUserMessage Send();
    }
}
