using TitanBot.Storage;

namespace TitanBot.Commands.Models
{
    public class CallPermission : IDbRecord
    {
        public ulong Id { get; set; }
        public ulong? GuildId { get; set; }
        public string CallName { get; set; }
        public ulong[] Roles { get; set; }
        public ulong? Permission { get; set; }
        public ulong[] Blacklisted { get; set; }
    }
}
