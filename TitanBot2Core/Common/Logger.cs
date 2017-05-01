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
        public event Func<LogEntry, Task> HandleLog;

        internal Task Log(LogEntry entry)
            => HandleLog?.Invoke(entry) ?? Task.CompletedTask;

        internal Task Log(Exception ex, string source)
            => Log(new LogEntry(LogType.Exception, ex.ToString(), source));

        internal Task Log(LogMessage msg)
            => Log(new LogEntry(LogType.Client, msg.Message ?? msg.Exception.Message, msg.Source));
    }

    public class LogEntry
    {
        public DateTime Time { get; private set; }
        public LogType Type { get; private set; }
        public string Description { get; private set; }
        public string Source { get; private set; }

        private string typeText;
        public string TypeText
        {
            get
            {
                if (Type == LogType.Other)
                    return typeText ?? "Other";
                return Type.ToString();
            }
        }

        internal LogEntry(LogType type, string description, string source)
        {
            Type = type;
            Description = description;
            Source = source;
            Time = DateTime.Now;
        }

        internal LogEntry(string type, string description, string source)
            : this(LogType.Other, description, source)
        {
            typeText = type;
        }

        public override string ToString()
        {
            return $"[{Time.ToString("HH:mm:ss")}][{TypeText}][{Source}]\t{Description}";
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
