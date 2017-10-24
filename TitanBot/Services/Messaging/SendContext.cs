using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;
using TitanBot.Core.Services.Messaging;
using TitanBot.Models;

namespace TitanBot.Services.Messaging
{
    internal class SendContext : ISendContext
    {
        private ITranslationService _transService;
        private IFormatterService _formatter;

        public IMessageChannel Channel { get; set; }

        public IUser User { get; set; }

        public IDisplayable<string> Text { get; private set; }

        public IDisplayable<IEmbedable> Embedable { get; private set; }

        public bool IsTTS { get; private set; }

        public IAttachmentStream Attachment { get; private set; }

        public SendContext(IMessageChannel channel, IUser user, ITranslationService transService, IFormatterService formatter)
        {
            _transService = transService;
            _formatter = formatter;
            Channel = channel;
            User = user;
        }

        public IUserMessage Send(RequestOptions options = null)
            => SendAsync(options).Result;

        public Task<IUserMessage> SendAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public ISendContext WithAttachment(IAttachmentStream attachment)
            => DelegateExtensions.RunInline(c => c.Attachment = attachment, this);

        public ISendContext WithAttachment(Stream fileStream, string fileName)
            => DelegateExtensions.RunInline(c => c.Attachment = AttachmentStream.From(fileStream, fileName), this);

        public ISendContext WithAttachment(string text, string fileName)
            => DelegateExtensions.RunInline(c => c.Attachment = AttachmentStream.From(text, fileName), this);

        public ISendContext WithAttachmentFile(string filePath, string fileName = null)
            => DelegateExtensions.RunInline(c => c.Attachment = AttachmentStream.FromFile(filePath, fileName), this);

        public ISendContext WithEmbedable(IEmbedable embedable)
            => DelegateExtensions.RunInline(c => c.Embedable = TransEmbedable.Wrap(embedable), this);

        public ISendContext WithEmbedable(IDisplayable<IEmbedable> embedable)
            => DelegateExtensions.RunInline(c => c.Embedable = embedable, this);

        public ISendContext WithKeyText(string key, params object[] values)
            => DelegateExtensions.RunInline(c => c.Text = TransKey.From(key, values), this);

        public ISendContext WithText(IDisplayable<string> text)
            => DelegateExtensions.RunInline(c => c.Text = text, this);

        public ISendContext WithText(string text)
            => DelegateExtensions.RunInline(c => c.Text = TransText.From(text), this);

        public ISendContext WithText(string format, params object[] values)
            => DelegateExtensions.RunInline(c => c.Text = TransText.From(format, values), this);

        public ISendContext WithText(Func<ITranslationSet, IValueFormatter, string> localisationFunc)
            => DelegateExtensions.RunInline(c => c.Text = TransFunc.From(localisationFunc), this);

        public ISendContext WithTTS(bool tts)
            => DelegateExtensions.RunInline(c => c.IsTTS = tts, this);
    }
}