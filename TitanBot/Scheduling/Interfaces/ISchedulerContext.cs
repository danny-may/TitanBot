using TitanBot.Commands;

namespace TitanBot.Scheduling
{
    public interface ISchedulerContext : IMessageContext
    {
        ISchedulerRecord Record { get; }
    }
}
