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
        public ulong InvitePermissions { get; set; } = 8;
        public ulong SuggestChannel { get; set; } = 312361037749944321;
        public ulong BugChannel { get; set; } = 312389178564542464;
        public ulong SubmissionChannel { get; set; } = 319164064414695429;
        public ulong? FocusId { get; set; } = null;
        public HighScoreSheetRef HighScoreSettings { get; set; } = new HighScoreSheetRef();
        public Dictionary<string, string> Versions { get; set; }
            = new Dictionary<string, string>
            {
                { "/ArtifactInfo.csv", "1.4" },
                { "/EquipmentInfo.csv", "1.4" },
                { "/PetInfo.csv", "1.4" },
                { "/SkillTreeInfo.csv", "1.4" }
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

        public class HighScoreSheetRef
        {
            public int DataStartRow { get; set; } = 4;
            public int RankingCol { get; set; } = 0;
            public int NameCol { get; set; } = 1;
            public int ClanCol { get; set; } = 2;
            public int RankMsCol { get; set; } = 3;
            public int SimMSCol { get; set; } = 4;
            public int RankRelCol { get; set; } = 5;
            public int TotalRelicsCol { get; set; } = 6;
            public int RawADCol { get; set; } = 7;
            public int FullADCol { get; set; } = 8;
        }
    }
}
