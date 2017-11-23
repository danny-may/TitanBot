using System;

namespace TitanBot.Storage
{
    public class Error : IDbRecord
    {
        public ulong Id { get; set; }
        public ulong? Channel { get; set; }
        public ulong? Message { get; set; }
        public ulong? User { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
    }
}