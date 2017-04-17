using Newtonsoft.Json;
using System;
using System.IO;

namespace TitanBot2.Common
{
    public class Resources
    {
        [JsonIgnore]
        private static string FileName { get; set; } = "config/resources.json";
        public Strings String { get; set; } = new Strings();
        public Emojis Emoji { get; set; } = new Emojis();

        public static Strings Str { get { return Instance.String; } }
        public static Emojis Emo { get { return Instance.Emoji; } }

        private static Resources _instance;
        private static Resources Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();
                return _instance;
            }
        }

        public static void Reload()
        {
            _instance = null;
        }

        private static Resources Load()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var resources = new Resources();

                resources.SaveJson();
                return resources;
            }
            return JsonConvert.DeserializeObject<Resources>(File.ReadAllText(file));
        }

        private void SaveJson()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public class Strings
        {
            public string ErrorText { get; private set; } = ":no_entry_sign: **Oops!**";
            public string SuccessText { get; private set; } = ":white_check_mark: **Got it!**";
            public string InfoText { get; private set; } = ":information_source:";

            internal Strings() { }
            
        }

        public class Emojis
        {

            public string No_entry_sign { get; private set; } = "http://emojipedia-us.s3.amazonaws.com/cache/74/95/7495a6b391014cb87772e381ab24d22a.png";
            public string White_check_mark { get; private set; } = "http://emojipedia-us.s3.amazonaws.com/cache/f7/cd/f7cd0427e5531771b69f6cce997cb872.png";
            public string Information_source { get; private set; } = "http://emojipedia-us.s3.amazonaws.com/cache/2f/e4/2fe4dd7dea335d509e7f03ac620847d8.png";

            internal Emojis() { }
        }
    }
}
