using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.SchedulerService
{
    public interface ISchedulerCallback
    {
        void Handle(ulong id, DateTime from, TimeSpan interval, DateTime to, DateTime current);
    }
}
