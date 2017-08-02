using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TitanBot.Formatting
{
    public class TextResourceManager : ITextResourceManager
    {
        private Dictionary<string, Dictionary<Locale, string>> TextMap { get; set; }
        static readonly string FileName = Path.Combine(AppContext.BaseDirectory, "Localisation.json");
        static readonly string DirectoryPath = Path.GetDirectoryName(FileName);
        private ValueFormatter ValueFormatter { get; }

        private Dictionary<string, Dictionary<Locale, string>> Defaults { get; } = new Dictionary<string, Dictionary<Locale, string>>();

        public Locale[] SupportedLanguages => TextMap.SelectMany(t => t.Value.Keys).Distinct().ToArray();

        public TextResourceManager(ValueFormatter valueFormatter)
        {
            ValueFormatter = valueFormatter;
            Load();
        }

        private string SanitiseKey(string key)
            => key.ToUpper().Replace(' ', '_');

        public void AddResource(string key, string text)
            => AddResource(key, Locale.DEFAULT, text);
        public void AddResource(string key, Locale language, string text)
        {
            key = SanitiseKey(key);
            if (!TextMap.ContainsKey(key))
                TextMap.Add(key, new Dictionary<Locale, string>());
            TextMap[key][language] = text;
        }

        public void SaveChanges()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            EnsureKeys();
            File.WriteAllText(FileName, JsonConvert.SerializeObject(TextMap.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value), Newtonsoft.Json.Formatting.Indented));
        }

        public void Load()
        {
            if (!File.Exists(FileName))
                SaveChanges();
            TextMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Locale, string>>>(File.ReadAllText(FileName));
            TextMap = TextMap.ToDictionary(v => SanitiseKey(v.Key), v => v.Value);
            SaveChanges();
        }

        public ITextResourceCollection GetForLanguage(Locale language, FormattingType format)
        {
            return new TextResourceCollection( GetLanguageCoverage(language), ValueFormatter, format,
                TextMap.SelectMany(k => k.Value.Select(v => (key: k.Key, language: v.Key, text: v.Value)))
                       .Where(v => v.language == language || v.language == Locale.DEFAULT)
                       .GroupBy(v => v.key)
                       .ToDictionary(v => v.Key, v => (v.FirstOrDefault(l => l.language == Locale.DEFAULT).text, v.FirstOrDefault(l => l.language == language).text))
                );
        }

        public double GetLanguageCoverage(Locale language)
        {
            var totalString = (double)TextMap.Count;
            var covered = (double)TextMap.Count(v => v.Value.ContainsKey(language));
            return covered / totalString;
        }

        private void EnsureKeys()
        {
            TextMap = TextMap ?? new Dictionary<string, Dictionary<Locale, string>>();
            var missing = Defaults.Where(d => !TextMap.ContainsKey(d.Key));
            foreach (var key in missing)
                TextMap[key.Key] = new Dictionary<Locale, string>();

            foreach (var key in Defaults)
                foreach (var vals in key.Value)
                    if (!TextMap[key.Key].ContainsKey(vals.Key))
                        TextMap[key.Key][vals.Key] = vals.Value;
        }

        public void RequireKeys(IReadOnlyDictionary<string, string> values)
        {
            values = values ?? new Dictionary<string, string>();
            foreach (var pair in values)
            {
                var key = SanitiseKey(pair.Key);
                if (!Defaults.ContainsKey(key))
                    Defaults[key] = new Dictionary<Locale, string>();
                Defaults[key][Locale.DEFAULT] = pair.Value;
            }

            SaveChanges();
        }
    }
}
