using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.Formatting
{
    public interface ITranslationSet
    {
        string this[string key] { get; }

        double Coverage { get; }

        Language Language { get; }

        string GetTranslation(string key, params string[] items);

        string GetTranslation(string key);
    }
}