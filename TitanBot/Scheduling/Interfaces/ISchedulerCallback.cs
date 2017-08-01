using System;
using TitanBot.Contexts;

namespace TitanBot.Scheduling
{
    public interface ISchedulerCallback
    {
        void Handle(ISchedulerContext context, DateTime eventTime);
        void Complete(ISchedulerContext context, bool wasCancelled);
    }
}
