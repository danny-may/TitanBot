using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Replying
{
    public delegate Task OnSendEventHandler(IReplyContext context, IUserMessage message);
    public delegate Task MessageSendErrorHandler(Exception ex, IMessageChannel channel, string text, IEmbedable embed);

    public interface IReplyContext
    {
        event OnSendEventHandler OnSend;

        IReplyContext WithMessage(ILocalisable<string> message);
        IReplyContext WithAttachment(Func<Stream> attachment, string name);
        IReplyContext WithEmbedable(IEmbedable embedable);
        IReplyContext WithErrorHandler(MessageSendErrorHandler handler);
        IReplyContext WithTTS(bool tts);
        IReplyContext WithRequestOptions(RequestOptions options);

        ValueTask<IUserMessage> SendAsync(bool stealthy = false);
        void Send(bool stealthy = false);
    }
}
