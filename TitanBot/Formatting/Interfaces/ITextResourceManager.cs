using System.Collections.Generic;

namespace TitanBot.Formatting
{
    public interface ITextResourceManager
    {
        void AddResource(string key, string text);
        void AddResource(string key, Locale language, string text);
        void SaveChanges();
        ITextResourceCollection GetForLanguage(Locale language, FormattingType format);
        double GetLanguageCoverage(Locale language);
        Locale[] SupportedLanguages { get; }
        void Load();

        void RequireKeys(IReadOnlyDictionary<string, string> values);
    }
}