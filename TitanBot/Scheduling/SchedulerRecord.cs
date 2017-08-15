using System;
using TitanBot.Storage;

namespace TitanBot.Scheduling
{
    sealed class SchedulerRecord : IDbRecord, ISchedulerRecord
    {
        public ulong Id { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? MessageId { get; set; }
        public ulong? ChannelId { get; set; }
        public ulong UserId { get; set; }
        public bool IsComplete => CompleteTime.HasValue;
        public DateTime? CompleteTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Interval { get; set; }
        public string Callback { get; set; }
        public string Data { get; set; }
    }
}
