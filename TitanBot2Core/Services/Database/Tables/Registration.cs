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
        public int MaxStage { get; set; } = 0;
        public string Message { get; set; } = null;
        public Uri[] Images { get; set; }
        public double Relics { get; set; } = 0;
        public int CQPerWeek { get; set; } = 0;
        public int Taps { get; set; } = 0;
        public DateTime ApplyTime { get; set; } = DateTime.Now;
        public DateTime EditTime { get; set; } = DateTime.Now;
    }
}
