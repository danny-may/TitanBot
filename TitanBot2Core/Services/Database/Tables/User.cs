using LiteDB;

namespace TitanBot2.Services.Database.Tables
{
    public class User
    {
        [BsonId]
        public ulong DiscordId { get; set; }
        public string SupportCode { get; set; }
        public bool CanSubmit { get; set; } = true;
    }
}
