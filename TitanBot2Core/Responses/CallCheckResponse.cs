using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Responses
{
    public struct CallCheckResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        private CallCheckResponse(bool success, string message)
        {
            IsSuccess = success;
            Message = message;
        }

        public static CallCheckResponse FromSuccess()
            => new CallCheckResponse(true, null);

        public static CallCheckResponse FromError(string reason)
            => new CallCheckResponse(false, reason);
    }
}
