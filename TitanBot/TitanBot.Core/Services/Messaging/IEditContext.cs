using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Services.Messaging
{
    public interface IEditContext
    {
        IUserMessage Message { get; }

        IUser User { get; }

        Optional<IDisplayable<string>> Text { get; }

        Optional<IDisplayable<IEmbedable>> Embedable { get; }

        IEditContext WithText(IDisplayable<string> text);

        IEditContext WithText(string text);

        IEditContext WithText(string format, params object[] values);

        IEditContext WithText(Func<ITranslationSet, IValueFormatter, string> localisationFunc);

        IEditContext WithKeyText(string key, params object[] values);

        IEditContext WithEmbedable(IEmbedable embedable);

        IEditContext WithEmbedable(IDisplayable<IEmbedable> embedable);

        void Edit(RequestOptions options = null);

        Task EditAsync(RequestOptions options = null);
    }
}