using System;
using System.IO;
using System.Threading.Tasks;
using TitanBotBase.Util;

namespace TitanBotBase.Logger
{
    public class TitanBotLogger : ILogger
    {
        private readonly string LogPath;
        private readonly object SyncLock = new object();
        private readonly LogSeverity LogLevel;

        public TitanBotLogger() : this($@".\logs\{FileUtil.GetTimestamp()}.log") { }
        public TitanBotLogger(string logLocation) : this(LogSeverity.Critical | LogSeverity.Info, logLocation) { }
        public TitanBotLogger(LogSeverity logLevel, string logLocation)
        {
            LogPath = FileUtil.GetAbsolutePath(logLocation);
            LogLevel = logLevel;
        }

        public virtual void Log(ILoggable entry)
        {
            if (!ShouldLog(entry.Severity))
                return;
            lock (SyncLock)
            {
                FileUtil.EnsureDirectory(LogPath);
                File.AppendAllText(LogPath, entry.ToString());
            }
        }

        protected virtual bool ShouldLog(LogSeverity severity)
            => (LogLevel & severity) != 0;

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
