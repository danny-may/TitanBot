using System;
using Titansmasher.Services.Logging.Interfaces;

namespace Titansmasher.Services.Logging

{
    internal class AreaLogger : IAreaLogger
    {
        #region Fields

        public string Area { get; }
        public ILoggerService Parent { get; }

        #endregion Fields

        #region Constructors

        public AreaLogger(ILoggerService parent, string area)
        {
            Parent = parent;
            Area = area;
        }

        #endregion Constructors

        #region IAreaLogger

        public void Log(LogLevel severity, string message)
            => Parent.Log(severity, Area, message);

        public void Log(Exception exception)
            => Parent.Log(Area, exception);

        public void Log(LogLevel severity, object message)
            => Parent.Log(severity, Area, message);

        #endregion IAreaLogger
    }
}