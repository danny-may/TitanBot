using System.Collections.Generic;
using TitanBot.Services.Database;

namespace TitanBot.Services.Setting
{
    public class SettingRecord : DbRecord
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }
}