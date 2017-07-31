using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.Scheduling
{
    public interface ISchedulerContext : IMessageContext
    {
        ISchedulerRecord Record { get; }
    }
}
