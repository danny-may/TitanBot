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
        private readonly object _syncLock = new object();
        private readonly LogSeverity _logLevel;

        public TitanBotLogger() : this($@".\logs\{FileUtil.GetTimestamp()}.log") { }
        public TitanBotLogger(string logLocation) : this(LogSeverity.Critical | LogSeverity.Info, logLocation) { }
        public TitanBotLogger(LogSeverity logLevel, string logLocation)
        {
            _logPath = FileUtil.GetAbsolutePath(logLocation);
            _logLevel = logLevel;
        }

        public virtual void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            lock (_syncLock)
            {
                FileUtil.EnsureDirectory(_logPath);
                File.AppendAllText(_logPath, entry.ToString());
            }
        }

        protected virtual bool ShouldLog(LogSeverity severity)
            => (_logLevel & severity) != 0;

        public void Log(LogSeverity severity, LogType type, string message, string source)
            => Log(new LogEntry(severity, type, message, source));

        public void Log(Exception ex, string source)
            => Log(new LogEntry(LogSeverity.Error, LogType.Exception, ex.ToString(), source));

        public Task LogAsync(ILoggable entry)
            => Task.Run(() => Log(entry));

        public Task LogAsync(LogSeverity severity, LogType type, string message, string source)
            => Task.Run(() => Log(severity, type, message, source));

        public Task LogAsync(Exception ex, string source)
            => Task.Run(() => Log(ex, source));
    }
}
