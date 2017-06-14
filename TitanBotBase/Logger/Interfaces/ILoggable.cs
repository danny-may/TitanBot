﻿using System;

namespace TitanBotBase.Logger
{
    public interface ILoggable
    {
        LogSeverity Severity { get; }
        LogType LogType { get; }
        string Message { get; }
        string Source { get; }
        DateTime LogTime { get; }
    }
}