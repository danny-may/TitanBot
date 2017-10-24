using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Services.Messaging
{
    internal class EditContext : IEditContext
    {
        private ITranslationService _transService;
        private IFormatterService _formatter;

        public IUserMessage Message { get; }

        public IUser User { get; }

        public Optional<IDisplayable<IEmbedable>> Embedable { get; private set; }

        public Optional<IDisplayable<string>> Text { get; private set; }

        public EditContext(IUserMessage message, IUser user, ITranslationService transService, IFormatterService formatter)
        {
            _transService = transService;
            _formatter = formatter;
            Message = message;
            User = user;
        }

        public void Edit(RequestOptions options = null)
            => EditAsync(options).Wait();

        public Task EditAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IEditContext WithEmbedable(IEmbedable embedable)
            => DelegateExtensions.RunInline(c => c.Embedable = TransEmbedable.Wrap(embedable), this);

        public IEditContext WithEmbedable(IDisplayable<IEmbedable> embedable)
            => DelegateExtensions.RunInline(c => c.Embedable = Optional.Create(embedable), this);

        public IEditContext WithKeyText(string key, params object[] values)
            => DelegateExtensions.RunInline(c => c.Text = TransKey.From(key, values), this);

        public IEditContext WithText(IDisplayable<string> text)
            => DelegateExtensions.RunInline(c => c.Text = Optional.Create(text), this);

        public IEditContext WithText(string text)
            => DelegateExtensions.RunInline(c => c.Text = TransText.From(text), this);

        public IEditContext WithText(string format, params object[] values)
            => DelegateExtensions.RunInline(c => c.Text = TransText.From(format, values), this);

        public IEditContext WithText(Func<ITranslationSet, IValueFormatter, string> localisationFunc)
            => DelegateExtensions.RunInline(c => c.Text = TransFunc.From(localisationFunc), this);
    }
}