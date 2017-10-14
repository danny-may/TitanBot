using TitanBot.Contexts;

namespace TitanBot.Scheduling
{
    public interface ISchedulerCallback
    {
        void Handle(ISchedulerContext context);
        void Complete(ISchedulerContext context, bool wasCancelled);
    }
}