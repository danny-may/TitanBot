using System;

namespace TitanBot.Logger
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
        {
            var core = $"[{LogTime}][{Severity}][{LogType}][{Source}]";
            var padding = "\n".PadRight(core.Length + 1);
            return $"{core}{Message.Replace("\n", padding)}";
        }
    }
}
