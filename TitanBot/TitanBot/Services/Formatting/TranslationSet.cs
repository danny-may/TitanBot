using System.Collections.Concurrent;
using System.Collections.Generic;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Services.Formatting
{
    public class TranslationSet : ITranslationSet
    {
        #region Fields

        private readonly ConcurrentDictionary<string, string> _translations;

        #endregion Fields

        #region Constructors

        public TranslationSet(Language language, IDictionary<string, string> translations, double coverage)
        {
            Language = language;
            _translations = new ConcurrentDictionary<string, string>(translations);
            Coverage = coverage;
        }

        #endregion Constructors

        #region ITranslationSet

        public string this[string key] => GetTranslation(key);

        public double Coverage { get; }

        public Language Language { get; }

        public string GetTranslation(string key, params string[] items)
            => string.Format(GetTranslation(key), items);

        public string GetTranslation(string key)
        {
            if (key == null)
                return null;
            key = key.ToUpper().Replace(' ', '_');
            if (_translations.TryGetValue(key, out var text))
                return text;
            return key;
        }

        #endregion ITranslationSet
    }
}