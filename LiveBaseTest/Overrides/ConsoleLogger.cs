using System;
using TitanBot.Logging;

namespace LiveBaseTest
{
    class ConsoleLogger : Logger
    {
        protected override LogSeverity LogLevel => LogSeverity.Critical | LogSeverity.Debug | LogSeverity.Error | LogSeverity.Info;

        protected override void WriteLog(ILoggable entry)
        {
            Console.WriteLine(entry);
            base.WriteLog(entry);
        }
    }
}
