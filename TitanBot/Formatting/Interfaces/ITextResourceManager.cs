namespace TitanBot.Formatting
{
    public interface ITextResourceManager
    {
        void AddResource(string key, Locale language, string text);
        ITextResourceCollection GetForLanguage(Locale language, ValueFormatter valueFormatter);
        double GetLanguageCoverage(Locale language);
        Locale[] SupportedLanguages { get; }
        void Refresh();
    }
}