using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Scheduler
{
    public abstract class Callback
    {
        public abstract EventCallback Handles { get; }
        public abstract Task Execute(TimerContext context);
    }
}
