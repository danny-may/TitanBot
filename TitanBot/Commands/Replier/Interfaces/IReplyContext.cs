using Discord;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public delegate Task OnSendEventHandler(IReplyContext context, IUserMessage message);
    public delegate Task MessageSendErrorHandler(Exception ex, IMessageChannel channel, IMessageContext context, string text, IEmbedable embed);

    public interface IReplyContext
    {
        event OnSendEventHandler OnSend;

        IReplyContext WithMessage(string message);
        IReplyContext WithMessage(string message, ReplyType replyType);
        IReplyContext WithMessage(string message, params object[] values);
        IReplyContext WithMessage(string message, ReplyType replyType, params object[] values);
        IReplyContext WithAttachment(Func<Stream> attachment, string name);
        IReplyContext WithEmbedable(IEmbedable embedable);
        IReplyContext WithErrorHandler(MessageSendErrorHandler handler);
        IReplyContext WithTTS(bool tts);
        IReplyContext WithRequestOptions(RequestOptions options);

        ValueTask<IUserMessage> SendAsync(bool stealthy = false);
        void Send(bool stealthy = false);
    }
}
