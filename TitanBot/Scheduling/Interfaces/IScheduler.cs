using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitanBot.Scheduling
{
    public interface IScheduler
    {
        bool Enabled { get; set; }
        ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = null, DateTime? to = null, ulong? message = null, ulong? channel = null, string data = null)
            where T : ISchedulerCallback;
        ISchedulerRecord[] Complete<T>(ulong? guildId, ulong? userId, bool wasCancelled = true)
            where T : ISchedulerCallback;
        ISchedulerRecord Complete(ulong id, bool wasCancelled = true);
        ISchedulerRecord[] Complete(IEnumerable<ulong> ids, bool wasCancelled = true);
        ValueTask<int> PruneBefore(DateTime date);
        int ActiveCount();
        ISchedulerRecord GetMostRecent<T>(ulong guildId)
            where T : ISchedulerCallback;
    }
}
