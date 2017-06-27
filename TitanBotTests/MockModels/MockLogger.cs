using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Logging;

namespace TitanBotBaseTest.MockModels
{
    public class MockLogger : ILogger
    {
        public void Log(LogSeverity severity, LogType type, string message, string source)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(LogSeverity severity, LogType type, string message, string source)
        {
            throw new NotImplementedException();
        }

        public void Log(ILoggable entry)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(ILoggable entry)
        {
            throw new NotImplementedException();
        }

        public void Log(Exception ex, string source)
        {
            throw new NotImplementedException();
        }

        public Task LogAsync(Exception ex, string source)
        {
            throw new NotImplementedException();
        }
    }
}
