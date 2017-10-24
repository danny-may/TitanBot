using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Models.Contexts
{
    public interface ITranslationContext
    {
        ITranslationService TranslationService { get; }
        IFormatterService FormatterService { get; }
        ITranslationSet TranslationSet { get; }
        IValueFormatter ValueFormatter { get; }
    }
}