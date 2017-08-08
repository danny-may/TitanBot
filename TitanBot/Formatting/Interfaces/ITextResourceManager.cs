using System.Collections.Generic;

namespace TitanBot.Formatting
{
    public interface ITextResourceManager
    {
        void AddResource(string key, string text);
        void AddResource(string key, Locale language, string text);
        void SaveChanges();
        ITextResourceCollection GetForLanguage(Locale language, FormatType format);
        double GetLanguageCoverage(Locale language);
        Locale[] SupportedLanguages { get; }
        void Reload();

        void RequireKeys(IReadOnlyDictionary<string, string> values);
        string Export(Locale language);
        void Import(Locale language, string jsonText);
        Dictionary<string, string> GetCurrent(Locale language);
    }
}