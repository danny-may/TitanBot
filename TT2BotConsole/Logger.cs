using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Logger;
using TitanBotBase.Util;

namespace TT2BotConsole
{
    class Logger : TitanBotLogger
    {
        public Logger() { }

        public Logger(LogSeverity logLevel, string logLocation) : base(logLevel, logLocation) { }

        public override void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            Console.Out.WriteLineAsync(entry.ToString());
            base.Log(entry);
        }
    }
}
