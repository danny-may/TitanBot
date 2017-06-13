using Newtonsoft.Json.Linq;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    public class GlobalSetting : IDbRecord
    {
        public ulong Id { get; set; } = 0;
        public ulong[] Owners { get; set; } = new ulong[0];
        public JObject AdditionalSettings { get; set; } = new JObject();
    }
}
