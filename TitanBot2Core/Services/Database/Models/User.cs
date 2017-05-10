using LiteDB;

namespace TitanBot2.Services.Database.Models
{
    public class User
    {
        [BsonId]
        public ulong DiscordId { get; set; }
        public string SupportCode { get; set; }
    }
}
