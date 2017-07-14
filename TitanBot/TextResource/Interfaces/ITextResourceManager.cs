namespace TitanBot.TextResource
{
    public interface ITextResourceManager
    {
        void AddResource(string key, Locale language, string text);
        ITextResourceCollection GetForLanguage(Locale language);
        double GetLanguageCoverage(Locale language);
        Locale[] SupportedLanguages { get; }
        void Refresh();
    }
}