using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Scheduler
{
    public interface IScheduler
    {
        bool IsRunning { get; }
        ulong Queue<T>(DateTime from)
            where T : ISchedulerCallback;
        ulong Queue<T>(DateTime from, TimeSpan period)
            where T : ISchedulerCallback;
        ulong Queue<T>(DateTime from, DateTime to)
            where T : ISchedulerCallback;
        ulong Queue<T>(DateTime from, TimeSpan period, DateTime to)
            where T : ISchedulerCallback;
        Task Cancel(ulong id);
        Task Cancel(IEnumerable<ulong> ids);
        Task StartAsync();
        Task StopAsync();
        Task<int> ActiveCount();
    }
}
