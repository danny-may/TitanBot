using System;
using TitanBot.Scheduling;

namespace TitanBot.Contexts
{
    public interface ISchedulerContext : IMessageContext
    {
        ISchedulerRecord Record { get; }
        DateTime CycleTime { get; }
        TimeSpan Delay { get; }
    }
}
