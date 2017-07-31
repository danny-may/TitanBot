using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitanBot.Scheduling
{
    public interface IScheduler
    {
        bool IsRunning { get; }
        ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = null, DateTime? to = null, ulong? message = null, ulong? channel = null, string data = null)
            where T : ISchedulerCallback;
        ISchedulerRecord[] Complete<T>(ulong? guildId, ulong? userId, bool wasCancelled = true)
            where T : ISchedulerCallback;
        ISchedulerRecord Complete(ulong id, bool wasCancelled = true);
        ISchedulerRecord[] Complete(IEnumerable<ulong> ids, bool wasCancelled = true);
        Task StartAsync();
        Task StopAsync();
        int ActiveCount();
        ISchedulerRecord GetMostRecent<T>(ulong guildId) 
            where T : ISchedulerCallback;
    }
}
