using System.Collections.Generic;

namespace TitanBot.Formatting
{
    public interface ITextResourceManager
    {
        void AddResource(string key, string text);
        void AddResource(string key, Locale language, string text);
        void SaveChanges();
        ITextResourceCollection GetForLanguage(Locale language, ValueFormatter valueFormatter);
        double GetLanguageCoverage(Locale language);
        Locale[] SupportedLanguages { get; }
        void Refresh();

        void RequireKeys(Dictionary<string, string> values);
    }
}