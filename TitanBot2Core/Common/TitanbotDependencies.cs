using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Common
{
    public class TitanbotDependencies
    {
        public TitanBot TitanBot { get; set; }
        public DiscordSocketClient Client { get; set; }
        public TitanbotDatabase Database { get; set; }
        public TimerService TimerService { get; set; }
        public Logger Logger { get; set; }
        public CachedWebService WebSerivce { get; set; }
    }
}
