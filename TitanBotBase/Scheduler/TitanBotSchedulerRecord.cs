using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Database;

namespace TitanBotBase.Scheduler
{
    sealed class TitanBotSchedulerRecord : IDbRecord
    {
        public ulong Id { get; set; }
        public bool Complete { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Interval { get; set; }
        public Type Callback { get; set; }
    }
}
