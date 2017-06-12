using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Logger
{
    public class LogEntry : ILoggable
    {
        public LogSeverity Severity { get; }

        public LogType LogType { get; }

        public string Message { get; }

        public string Source { get; }

        public DateTime LogTime { get; }

        public LogEntry(LogSeverity severity, LogType type, string message, string source)
        {
            Severity = severity;
            LogType = type;
            Message = message;
            Source = source;
            LogTime = DateTime.Now;
        }

        public override string ToString()
            => $"[{LogTime}][{Severity}][{LogType}][{Source}]{Message}";
    }
}
