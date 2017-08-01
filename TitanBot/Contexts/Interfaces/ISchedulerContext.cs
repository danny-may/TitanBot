using TitanBot.Scheduling;

namespace TitanBot.Contexts
{
    public interface ISchedulerContext : IMessageContext
    {
        ISchedulerRecord Record { get; }
    }
}
