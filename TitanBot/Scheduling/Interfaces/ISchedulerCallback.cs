using TitanBot.Contexts;

namespace TitanBot.Scheduling
{
    public interface ISchedulerCallback
    {
        bool Handle(ISchedulerContext context);
        void Complete(ISchedulerContext context, bool wasCancelled);
    }
}