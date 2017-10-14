using System;
using System.Threading.Tasks;

namespace TitanBot.Scheduling
{
    public interface IScheduler
    {
        bool Enabled { get; set; }
        ulong Queue<T>(ulong userId, ulong? guildID, DateTime from, TimeSpan? period = null, DateTime? to = null, ulong? message = null, ulong? channel = null, object data = null)
            where T : ISchedulerCallback;
        ISchedulerRecord[] Cancel<T>(ulong? guildId, ulong? userId, Func<string, bool> predicate = null)
            where T : ISchedulerCallback;
        ISchedulerRecord Cancel(ulong id);
        ISchedulerRecord[] Cancel(ulong[] ids);
        ValueTask<int> PruneBefore(DateTime date);
        int ActiveCount();
        ISchedulerRecord GetMostRecent<T>(ulong? guildId, ulong? userId, Func<string, bool> predicate = null)
            where T : ISchedulerCallback;
        void PreRegister<T>(T handler)
            where T : ISchedulerCallback;
    }
}