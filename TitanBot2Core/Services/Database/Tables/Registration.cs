using LiteDB;
using System;

namespace TitanBot2.Services.Database.Tables
{
    public class Registration
    {
        [BsonId]
        public int Id { get; set; }
        public ulong? GuildId { get; set; }
        public ulong UserId { get; set; }
        public int MaxStage { get; set; }
        public string Message { get; set; }
        public Uri[] Images { get; set; }
        public int Relics { get; set; }
        public int CQPerWeek { get; set; }
        public int Taps { get; set; }
        public DateTime ApplyTime { get; set; } = DateTime.Now;
        public DateTime EditTime { get; set; } = DateTime.Now;
    }
}
