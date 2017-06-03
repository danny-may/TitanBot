using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Logger
{
    public enum LogSeverity
    {
        Critical = 1,
        Error = 2,
        Debug = 4,
        Info = 8,
        Verbose = 16,
    }
}
