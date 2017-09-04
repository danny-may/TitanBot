using Discord;
using System.Collections.Generic;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.Formatting
{
    public interface ITranslationService
    {
        void AddResource(string key, string text);

        void AddResource(string key, Language language, string text);

        void Import(Language language, string json);

        string Export(Language language);

        void Persist();

        void Reload();

        ITranslationSet GetLanguage(Language language);

        ITranslationSet GetLanguage(IUser user);

        double CoverageOf(Language language);

        void RegisterKeys(IDictionary<string, string> keys);
    }
}