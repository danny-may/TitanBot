using System.Collections.Generic;
using TitanBot.Storage;

namespace TitanBot.Settings
{
    public class Setting : IDbRecord
    {
        public ulong Id { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
        public string Serialized { get; set; }
    }
}