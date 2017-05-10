using System;

namespace TitanBot2.Services.Database.Models
{
    public class Registration
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public int MaxStage { get; set; }
        public string Message { get; set; }
        public string[] Images { get; set; }
        public DateTime ApplyTime { get; set; } = DateTime.Now;
        public DateTime EditTime { get; set; } = DateTime.Now;
    }
}
