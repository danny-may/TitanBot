using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Services.Messaging
{
    public interface ISendContext
    {
        IMessageChannel Channel { get; }

        IUser User { get; }

        IDisplayable<string> Text { get; }

        IDisplayable<IEmbedable> Embedable { get; }

        bool IsTTS { get; }

        IAttachmentStream Attachment { get; }

        ISendContext WithText(IDisplayable<string> text);

        ISendContext WithText(string text);

        ISendContext WithText(string format, params object[] values);

        ISendContext WithText(Func<ITranslationSet, IValueFormatter, string> localisationFunc);

        ISendContext WithKeyText(string key, params object[] values);

        ISendContext WithEmbedable(IEmbedable embedable);

        ISendContext WithEmbedable(IDisplayable<IEmbedable> embedable);

        ISendContext WithTTS(bool tts);

        ISendContext WithAttachment(IAttachmentStream attachment);

        ISendContext WithAttachment(Stream fileStream, string fileName);

        ISendContext WithAttachment(string text, string fileName);

        ISendContext WithAttachmentFile(string filePath, string fileName = null);

        Task<IUserMessage> SendAsync(RequestOptions options = null);

        IUserMessage Send(RequestOptions options = null);
    }
}