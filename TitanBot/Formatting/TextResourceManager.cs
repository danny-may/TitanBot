using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private Dictionary<string, string> Defaults { get; } = new Dictionary<string, string>();

        public Locale[] SupportedLanguages => TextMap.SelectMany(t => t.Value.Keys).Distinct().ToArray();

        public TextResourceManager(ValueFormatter valueFormatter)
        {
            ValueFormatter = valueFormatter;
            Reload();
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

        public void Import(Locale language, string jsontext)
        {
            var obj = JObject.Parse(jsontext);
            var deserialised = Deserialise(obj);

            foreach (var entry in deserialised)
                AddResource(entry.Key, language, entry.Value);

            SaveChanges();
        }

        public string Export(Locale language)
            => JsonConvert.SerializeObject(Serialise(GetCurrent(language)), Newtonsoft.Json.Formatting.Indented);

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

        public Dictionary<string, string> Deserialise(JObject obj)
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

        public Dictionary<string, string> GetCurrent(Locale language)
            => Defaults.ToDictionary(k => k.Key, v => TextMap.TryGetValue(v.Key, out var vals) && vals.TryGetValue(language, out var text) ? text : v.Value);

        public void SaveChanges()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            File.WriteAllText(FileName, JsonConvert.SerializeObject(TextMap.OrderBy(k => k.Key).ToDictionary(k => k.Key, k => k.Value), Newtonsoft.Json.Formatting.Indented));
        }

        public void Reload()
        {
            if (File.Exists(FileName))
                TextMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Locale, string>>>(File.ReadAllText(FileName));
            TextMap = TextMap?.ToDictionary(v => SanitiseKey(v.Key), v => v.Value) ?? new Dictionary<string, Dictionary<Locale, string>>();
            SaveChanges();
        }

        public ITextResourceCollection GetForLanguage(Locale language, FormatType format)
            => new TextResourceCollection(GetLanguageCoverage(language), ValueFormatter, format, GetCurrent(language));

        public double GetLanguageCoverage(Locale language)
        {
            if (language == Locale.DEFAULT)
                return 1;
            var totalString = (double)Defaults.Count;
            var covered = (double)TextMap.Count(v => v.Value.ContainsKey(language));
            return covered / totalString;
        }

        public void RegisterKeys(IReadOnlyDictionary<string, string> values)
        {
            values = values ?? new Dictionary<string, string>();
            foreach (var pair in values)
            {
                var key = SanitiseKey(pair.Key);
                Defaults[key] = pair.Value;
            }
        }
    }
}
