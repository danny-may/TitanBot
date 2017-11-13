using LiteDB;
using Newtonsoft.Json;
using System.IO;
using Titansmasher.Utilities.Extensions;

namespace Titanbot.Core
{
    public class Configuration
    {
        #region Statics

        public static FileInfo Location = new FileInfo("./config.json");

        public static Configuration Load()
            => Load<Configuration>();

        public static TConfiguation Load<TConfiguation>()
            where TConfiguation : Configuration, new()
        {
            Location.EnsureExists("{}");
            return JsonConvert.DeserializeObject<TConfiguation>(Location.ReadAllText());
        }

        #endregion Statics

        #region Fields

        #region Database

        public string Database_Location { get; set; } = "./database/bot.db4";
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

        public Configuration()
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