using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.TextResource
{
    public class TextResourceManager : ITextResourceManager
    {
        private Dictionary<string, Dictionary<Language, string>> TextMap { get; set; }

        public void AddResource(string key, Language language, string text)
        {
            if (!TextMap.ContainsKey(key))
                TextMap.Add(key, new Dictionary<Language, string>());
            TextMap[key][language] = text;
        }

        string FileName = "res\\text.json";

        public TextResourceManager()
        {
            Refresh();
        }

        public void Refresh()
        {

            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                LoadDefaults();
                File.WriteAllText(file, JsonConvert.SerializeObject(TextMap, Formatting.Indented));
            }
            TextMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Language, string>>>(File.ReadAllText(file));
        }

        public ITextResourceCollection GetForLanguage(Language language)
        {
            ;
            return new TextResourceCollection(
                TextMap.SelectMany(k => k.Value.Select(v => (key: k.Key, language: v.Key, text: v.Value)))
                       .Where(v => v.language == language || v.language == Language.DEFAULT)
                       .GroupBy(v => v.key)
                       .ToDictionary(v => v.Key, v => (v.FirstOrDefault(l => l.language == Language.DEFAULT).text, v.FirstOrDefault(l => l.language == language).text)));
        }

        private void LoadDefaults()
        {
            TextMap = new Dictionary<string, Dictionary<Language, string>>();

            AddResource("PING_INITIAL", Language.DEFAULT, "| ~{0} ms");
            AddResource("PING_INITIAL", Language.EN, "| ~{0} ms");
            AddResource("PING_VERIFY", Language.DEFAULT, "| {0} ms");
            AddResource("PING_VERIFY", Language.EN, "| {0} ms");
        }
    }
}
