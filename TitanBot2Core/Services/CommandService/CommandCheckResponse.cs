using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.CommandService
{
    public class CommandCheckResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        private CommandCheckResponse(bool success, string message)
        {
            IsSuccess = success;
            Message = message;
        }

        public static CommandCheckResponse FromSuccess()
            => new CommandCheckResponse(true, null);

        public static CommandCheckResponse FromError(string reason)
            => new CommandCheckResponse(false, reason);
    }
}
