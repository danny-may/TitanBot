using Discord;
using System;

namespace TitanBot2.Common
{
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
            :this(LogType.Other, description, source)
        {
            typeText = type;
        }

        public override string ToString()
        {
            return $"[{Time.ToString("HH:mm:ss")}][{TypeText}][{Source}]\t{Description}";
        }

        public static LogEntry FromException(Exception ex, string source)
        {
            return new LogEntry(LogType.Exception, ex.ToString(), source);
        }

        public static LogEntry FromClientLog(LogMessage msg)
        {
            return new LogEntry(LogType.Client, msg.Message, msg.Source);
        }
    }

    public enum LogType
    {
        Message,
        Command,
        Exception,
        Client,
        Handler,
        Other
    }
}
