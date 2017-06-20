using System;
using TitanBot.Database;

namespace TitanBot.Scheduler
{
    sealed class TitanBotSchedulerRecord : IDbRecord, ISchedulerRecord
    {
        public ulong Id { get; set; }
        public ulong? GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool Complete { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Interval { get; set; }
        public string Callback { get; set; }
        public string Data { get; set; }
    }
}
