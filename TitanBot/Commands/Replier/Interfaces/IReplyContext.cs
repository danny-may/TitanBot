using Discord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public delegate Task OnSendEventHandler(IReplyContext context, IUserMessage message);
    public delegate Task MessageErrorHandler(Exception ex, IMessageChannel channel, ICommandContext context, string text, IEmbedable embed);

    public interface IReplyContext
    {
        event OnSendEventHandler OnSend;

        IReplyContext WithMessage(string message);
        IReplyContext WithMessage(string message, ReplyType replyType);
        IReplyContext WithMessage(string message, params object[] values);
        IReplyContext WithMessage(string message, ReplyType replyType, params object[] values);
        IReplyContext WithAttachment(Func<Stream> attachment, string name);
        IReplyContext WithEmbedable(IEmbedable embedable);
        IReplyContext WithErrorHandler(MessageErrorHandler handler);
        IReplyContext WithTTS(bool tts);
        IReplyContext WithRequestOptions(RequestOptions options);

        Task<IUserMessage> SendAsync(bool stealthy = false);
        IUserMessage Send(bool stealthy = false);
    }
}
