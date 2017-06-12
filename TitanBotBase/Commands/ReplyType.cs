using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands
{
    public enum ReplyType : int
    {
        Success = 0,
        Error = 1,
        Info = 2,
        None = -1
    }
}
