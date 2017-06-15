using System;
using System.Threading.Tasks;

namespace TitanBotBase.Scheduler
{
    public interface ISchedulerCallback
    {
        void Handle(ISchedulerRecord record, DateTime eventTime);
        void Complete(ISchedulerRecord record, bool wasCancelled);
    }
}
