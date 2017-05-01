using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TitanBot2.Common
{
    public class Configuration
    {
        [JsonIgnore]
        public static string FileName { get; private set; } = "config/configuration.json";
        public ulong[] Owners { get; set; }
        public string Prefix { get; set; } = "t$";
        public string Token { get; set; } = "";
        public string ShutdownReason { get; set; } = null;
        public ulong InvitePermissions { get; set; } = 
        public Dictionary<string, float> Versions { get; set; }
            = new Dictionary<string, float>
            {
                { "/ArtifactInfo.csv", 1.0f }
            };

        private static Configuration _instance;
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();
                return (Configuration)_instance.MemberwiseClone();
            }
        }

        private static void EnsureExists()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new Configuration();

                config.SaveJson();
            }
        }
        
        public void SaveJson()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
            _instance = this;
        }

        public static void Reload()
        {
            _instance = null;
        }
        
        private static Configuration Load()
        {
            EnsureExists();
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }
    }
}
