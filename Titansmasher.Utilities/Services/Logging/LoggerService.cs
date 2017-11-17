using System;
using System.IO;
using System.Text;
using Titansmasher.Extensions;
using Titansmasher.Services.Logging.Interfaces;
using Titansmasher.Utilities;

namespace Titansmasher.Services.Logging

{
    public class LoggerService : ILoggerService
    {
        #region Fields

        public FileInfo Location { get; }
        public string TimestampStyle { get; set; } = "HH:MM:ss.ffff";

        private SynchronisedExecutor _processor = new SynchronisedExecutor();

        public LogLevel Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                Log(LogLevel.Always, $"Logger scope set to {Enum.GetName(typeof(LogLevel), value)}");
            }
        }

        private LogLevel _scope = LogLevel.Debug;

        #endregion Fields

        #region Constructors

        public LoggerService(string location, bool timestamp = true)
        {
            Location = new FileInfo(location ?? throw new ArgumentNullException(nameof(location)));
            if (timestamp)
                Location = Location.WithTimestamp();

            Location.EnsureExists();

            OnLog += LogToFile;
            OnLog += Console.Write;

            Log(LogLevel.Info, "Initialised");
        }

        public LoggerService() : this("./logs/log.txt")
        {
        }

        #endregion Constructors

        #region Methods

        private void LogToFile(string line)
        {
            lock (this)
                Location.AppendAllText(line);
        }

        #endregion Methods

        #region ILogger

        public event Action<string> OnLog;

        public void Log(LogLevel severity, string message, string area = null)
        {
            area = area ?? StackUtil.GetCallerClass().Name;

            if (severity > Scope)
                return;

            var builder = new StringBuilder();
            builder.Append($"[{DateTime.UtcNow.ToString(TimestampStyle)}]");
            builder.Append($"[{Enum.GetName(typeof(LogLevel), severity)}]");
            builder.Append($"[{area}] ");
            builder.Append(message);
            builder.Append(Environment.NewLine);

            var logString = builder.ToString();

            _processor.Run(() => OnLog(logString));
        }

        public void Log(LogLevel severity, object message, string area = null)
            => Log(severity, message.ToString(), area);

        public void Log(Exception exception, string area = null)
            => Log(LogLevel.Critical, exception.ToString(), area);

        public void ClearOld(DateTime before = default(DateTime))
            => Location.Directory.CleanDirectory(before);

        #endregion ILogger
    }
}