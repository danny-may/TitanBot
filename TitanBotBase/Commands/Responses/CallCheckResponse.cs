using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.Responses
{
    public struct CallCheckResponse
    {
        public CallCheckResult SuccessStatus { get; }
        public CallInfo[] PermittedCalls { get; }
        public string ErrorMessage { get; }

        private CallCheckResponse(CallCheckResult result, CallInfo[] calls, string message)
        {
            SuccessStatus = result;
            PermittedCalls = calls;
            ErrorMessage = message ?? "No errors given";
        }

        public static CallCheckResponse FromSuccess(CallInfo[] calls)
            => new CallCheckResponse(CallCheckResult.Successful, calls, null);
        public static CallCheckResponse FromError(CallCheckResult result, string message)
            => new CallCheckResponse(result, null, message);
    }

    public enum CallCheckResult
    {
        Successful,
        InvalidContext,
    }
}
