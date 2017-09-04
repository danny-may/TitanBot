using System;
using TitanBot.Core.Services.Database;

namespace TitanBot.Core.Services.Scheduler
{
    public interface ISchedulerRecord : IDbRecord<ulong>
    {
        bool IsComplete { get; }
        DateTime? CompleteTime { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
        TimeSpan? Interval { get; set; }
        string Callback { get; set; }
        string Payload { get; set; }
    }
}
