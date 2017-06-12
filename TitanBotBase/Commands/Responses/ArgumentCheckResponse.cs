using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.Responses
{
    struct ArgumentCheckResponse
    {
        public ArgumentCheckResult SuccessStatus { get; }
        public object[] ParsedArguments { get; }
        public object[] ParsedfFlags { get; }
        public string ErrorMessage { get; }

        private ArgumentCheckResponse(ArgumentCheckResult result, object[] args, object[] flags, string message)
        {
            SuccessStatus = result;
            ParsedArguments = args;
            ParsedfFlags = flags;
            ErrorMessage = message ?? "No errors given";
        }

        public static ArgumentCheckResponse FromSuccess(object[] arguments, object[] flags)
            => new ArgumentCheckResponse(ArgumentCheckResult.Successful, arguments, flags, null);
        public static ArgumentCheckResponse FromError(ArgumentCheckResult result, string message)
            => new ArgumentCheckResponse(result, null, null, message);
    }

    enum ArgumentCheckResult
    {
        Successful,
        InvalidArguments,
        Other,
        InvalidSubcall
    }
}
