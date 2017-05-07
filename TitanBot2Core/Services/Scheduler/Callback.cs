using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Scheduler
{
    public abstract class Callback
    {
        public EventCallback Handles { get; protected set; }
        public abstract Task Execute(TimerContext context);
    }
}
