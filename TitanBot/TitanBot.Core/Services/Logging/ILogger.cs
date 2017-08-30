using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Logging
{
    public interface ILogger
    {
        void Log(ILoggable entry);

        void Log(Exception error);

        void Log(Exception error, string source);
    }
}