using LiteDB;
using Newtonsoft.Json;
using System.IO;
using Titansmasher.Extensions;

namespace Titanbot.Core.Configuration
{
    public class Config
    {
        #region Statics

        public static FileInfo Location = new FileInfo("./config.json");

        public static Config Load()
            => Load<Config>();

        public static TConfig Load<TConfig>()
            where TConfig : Config, new()
        {
            Location.EnsureExists("{}");
            return JsonConvert.DeserializeObject<TConfig>(Location.ReadAllText());
        }

        #endregion Statics

        #region Fields

        #region Database

        public string Database_Location { get; set; } = "./database/titanbot.db4";
        public byte Database_LogLevel { get; set; } = Logger.FULL;

        #endregion Database

        #region Discord

        public string Discord_Token { get; set; } = "";

        #endregion Discord

        #region Commands

        public string Commands_DefaultPrefix { get; set; } = "t$";

        #endregion Commands

        #endregion Fields

        #region Constructors

        public Config()
        {
        }

        #endregion Constructors

        #region Methods

        public void Save(string location = null)
        {
            Location.EnsureDirectory();
            Location.WriteAllText(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        #endregion Methods
    }
}