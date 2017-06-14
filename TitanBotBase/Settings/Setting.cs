using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    class Setting : IDbRecord
    {
        public ulong Id { get; set; }
        public string Type { get; set; }
        public string Serialized { get; set; }
    }
}
