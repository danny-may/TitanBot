namespace TitanBot.TextResource
{
    public interface ITextResourceManager
    {
        void AddResource(string key, Language language, string text);
        ITextResourceCollection GetForLanguage(Language language);
        void Refresh();
    }
}