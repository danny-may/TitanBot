using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Services.Formatting
{
    public class TranslationService : ITranslationService
    {
        #region Statics

        public static string FileName = "./Localisation.json";

        private static FileInfo _file => new FileInfo(Path.Combine(AppContext.BaseDirectory, FileName));

        #endregion Statics

        #region Fields

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Language, string>> _translationMap = new ConcurrentDictionary<string, ConcurrentDictionary<Language, string>>();
        private readonly ConcurrentDictionary<string, string> _defaults = new ConcurrentDictionary<string, string>();

        public Language[] KnownLanguages => new Language[] { Language.DEFAULT }.Concat(_translationMap.SelectMany(s => s.Value.Keys))
                                                                               .Distinct()
                                                                               .ToArray();

        #endregion Fields

        #region Constructors

        public TranslationService()
        {
        }

        #endregion Constructors

        #region Methods

        private string FormatKey(string key)
            => key.ToUpper().Replace(' ', '_');

        private JObject Serialise(Dictionary<string, string> values)
        {
            var grouped = values.GroupBy(v => v.Key?.Split('_').First());
            var obj = new JObject();
            foreach (var group in grouped)
            {
                if (group.Count() > 1 || (group.Count() == 1 && group.First().Key.Split('_').Length > 1))
                    obj.Add(group.Key, Serialise(group.ToDictionary(k => k.Key.Substring(group.Key.Length).TrimStart('_'), k => k.Value)));
                else if (group.Count() == 1)
                    if (group.First().Key == "")
                        obj.Add("*", group.First().Value);
                    else
                        obj.Add(group.First().Key, group.First().Value);
            }

            return obj;
        }

        private Dictionary<string, string> Deserialise(JObject obj)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in obj)
            {
                if (item.Value is JObject jobj)
                    foreach (var entry in Deserialise(jobj))
                        dict[$"{item.Key}_{entry.Key}".TrimEnd('*', '_')] = entry.Value;
                else
                    dict[item.Key] = (string)item.Value;
            }
            return dict;
        }

        private Dictionary<string, string> GetCurrent(Language language)
            => _defaults.ToDictionary(k => k.Key, v => _translationMap.TryGetValue(v.Key, out var vals) && vals.TryGetValue(language, out var text) ? text : v.Value);

        #endregion Methods

        #region ITranslationService

        public void AddResource(string key, string text)
            => AddResource(key, Language.DEFAULT, text);

        public void AddResource(string key, Language language, string text)
            => _translationMap.GetOrAdd(FormatKey(key), k => new ConcurrentDictionary<Language, string>())[language] = text;

        public double CoverageOf(Language language)
            => language == Language.DEFAULT ? 1 : (double)_defaults.Count / _translationMap.Count(v => v.Value.ContainsKey(language));

        public string Export(Language language)
            => Serialise(GetCurrent(language)).ToString(Newtonsoft.Json.Formatting.Indented);

        public ITranslationSet GetLanguage(Language language)
        {
            throw new System.NotImplementedException();
        }

        public void Import(Language language, string json)
        {
            foreach (var entry in Deserialise(JObject.Parse(json)))
                AddResource(entry.Key, language, entry.Value);
            Persist();
        }

        public void Persist()
        {
            FileExtensions.EnsureFile(_file);
            File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(_translationMap.OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.Value)));
        }

        public void Reload()
        {
            FileExtensions.EnsureFile(_file);
            lock (_translationMap)
            {
                _translationMap.Clear();
                var tempMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Language, string>>>(File.ReadAllText(_file.FullName));
                foreach (var key in tempMap.Keys)
                {
                    _translationMap[FormatKey(key)] = new ConcurrentDictionary<Language, string>();
                    foreach (var language in tempMap[key])
                        _translationMap[FormatKey(key)][language.Key] = language.Value;
                }
            }
            Persist();
        }

        public void RegisterKeys(IDictionary<string, string> definitions)
        {
            definitions = definitions ?? new Dictionary<string, string>();
            foreach (var definition in definitions)
                _defaults[FormatKey(definition.Key)] = definition.Value;
        }

        public ITranslationSet GetLanguage(IUser user)
        {
            throw new NotImplementedException();
        }

        #endregion ITranslationService
    }
}