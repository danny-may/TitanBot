using System;
using System.IO;
using System.Text;
using Titanbot.Core.Logging.Interfaces;
using Titansmasher.Utilities.Extensions;
using Titansmasher.Utilities.Utilities;

namespace Titanbot.Core.Logging
{
    public class Logger : ILogger
    {
        #region Fields

        public FileInfo Location { get; }
        public string TimestampStyle { get; set; } = "HH:MM:ss.ffff";

        private SynchronisedExecutor _processor = new SynchronisedExecutor();

        public LogSeverity Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                _logger.Log(LogSeverity.Always, $"Logger scope set to {Enum.GetName(typeof(LogSeverity), value)}");
            }
        }

        private LogSeverity _scope;

        private IAreaLogger _logger;

        #endregion Fields

        #region Constructors

        public Logger(string location, bool timestamp = true)
        {
            Location = new FileInfo(location);
            if (timestamp)
                Location = Location.WithTimestamp();

            Location.EnsureExists();

            OnLog += LogToFile;

            _logger = CreateAreaLogger<Logger>();

            _logger.Log(LogSeverity.Info, "Initialised");
        }

        public Logger() : this("./logs/log.txt")
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

        public void Log(LogSeverity severity, string area, string message)
        {
            if ((severity & Scope) != severity)
                return;

            var builder = new StringBuilder();
            builder.Append($"[{DateTime.UtcNow.ToString(TimestampStyle)}]");
            builder.Append($"[{Enum.GetName(typeof(LogSeverity), severity)}]");
            builder.Append($"[{area}] ");
            builder.Append(message);
            builder.Append(Environment.NewLine);

            var logString = builder.ToString();

            _processor.Run(() => OnLog(logString));
        }

        public void Log(LogSeverity severity, string area, object message)
            => Log(severity, area, message.ToString());

        public void Log(string area, Exception exception)
            => Log(LogSeverity.Critical, area, exception.ToString());

        public IAreaLogger CreateAreaLogger<TArea>()
            => new AreaLogger(this, typeof(TArea).Name);
        public IAreaLogger CreateAreaLogger(string area)
            => new AreaLogger(this, area);

        #endregion ILogger
    }
}