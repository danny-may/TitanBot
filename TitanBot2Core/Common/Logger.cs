using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Common
{
    public class Logger
    {
        public event Func<ILoggable, Task> HandleLog;

        internal Task Log(ILoggable entry)
            => HandleLog?.Invoke(entry) ?? Task.CompletedTask;

        internal Task Log(Exception ex, string source)
            => Log(new BotLog(LogType.Exception, LogSeverity.Error, ex.ToString(), source));

        internal Task Log(LogMessage msg)
            => Log(new BotLog(LogType.Client, msg.Severity, msg.Message ?? msg.Exception.Message, msg.Source));
    }

    public interface ILoggable
    {
        DateTime LogTime { get; }
        LogSeverity Severity { get; }
        LogType LogType { get; }
        string Source { get; }
        string Message { get; }
    }

    internal class BotLog : ILoggable
    {
        public DateTime LogTime { get; private set; }
        public LogType LogType { get; private set; }
        public LogSeverity Severity { get; private set; }
        public string Message { get; private set; }
        public string Source { get; private set; }

        private string typeText;
        public string TypeText
        {
            get
            {
                if (LogType == LogType.Other)
                    return typeText ?? "Other";
                return LogType.ToString();
            }
        }

        internal BotLog(LogType type, LogSeverity severity, string description, string source)
        {
            LogType = type;
            Severity = severity;
            Message = description;
            Source = source;
            LogTime = DateTime.Now;
        }

        internal BotLog(string type, string description, string source)
            : this(LogType.Other, LogSeverity.Info, description, source)
        {
            typeText = type;
        }

        public override string ToString()
        {
            return $"[{LogTime.ToString("HH:mm:ss")}][{TypeText}][{Source}]\t{Message}";
        }
    }

    public enum LogType
    {
        Message,
        Command,
        Exception,
        Client,
        Handler,
        Service,
        Other
    }
}
