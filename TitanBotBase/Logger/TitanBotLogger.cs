using TitanBotBase.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace TitanBotBase.Logger
{
    public class TitanBotLogger : ILogger
    {
        private readonly string _logPath;
        private readonly object _syncLock;

        public TitanBotLogger(string logLocation)
        {
            _logPath = logLocation;
        }

        public void Log(ILoggable entry)
        {
            lock (_syncLock)
            {
                FileUtil.EnsureExists(_logPath);
                File.AppendAllText(_logPath, $"[{entry.LogTime}][{entry.Severity}][{entry.LogType}][{entry.Source}]{entry.Message}");
            }
        }

        public void Log(LogSeverity severity, LogType type, string message, string source)
            => Log(new LogEntry(severity, type, message, source));

        public void Log(Exception ex, string source)
            => Log(new LogEntry(LogSeverity.Error, LogType.Exception, ex.ToString(), source));

        public void Log(LogMessage msg)
            => Log(new LogEntry(LogSeverity.Critical, LogType.Client, msg.Message, msg.Source));

        public Task LogAsync(ILoggable entry)
            => Task.Run(() => Log(entry));

        public Task LogAsync(LogSeverity severity, LogType type, string message, string source)
            => Task.Run(() => Log(severity, type, message, source));

        public Task LogAsync(Exception ex, string source)
            => Task.Run(() => Log(ex, source));

        public Task LogAsync(LogMessage msg)
            => Task.Run(() => Log(msg));
    }
}
