using System;

namespace TitanBotBase.Scheduler
{
    public interface ISchedulerCallback
    {
        void Handle(ulong id, DateTime from, TimeSpan interval, DateTime to, DateTime current);
    }
}
