using System;
using Titanbot.Core.Logging.Interfaces;

namespace Titanbot.Core.Logging
{
    internal class AreaLogger : IAreaLogger
    {
        #region Fields

        private string _area;
        private ILogger _parent;

        #endregion Fields

        #region Constructors

        public AreaLogger(ILogger parent, string area)
        {
            _parent = parent;
            _area = area;
        }

        #endregion Constructors

        #region IAreaLogger

        public void Log(LogSeverity severity, string message)
            => _parent.Log(severity, _area, message);

        public void Log(Exception exception)
            => _parent.Log(_area, exception);

        public void Log(LogSeverity severity, object message)
            => _parent.Log(severity, _area, message);

        #endregion IAreaLogger
    }
}