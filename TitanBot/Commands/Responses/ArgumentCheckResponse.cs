using System;
using System.Linq;
using TitanBot.TextResource;

namespace TitanBot.Commands.Responses
{
    struct ArgumentCheckResponse
    {
        public ArgumentCheckResult SuccessStatus { get; }
        public object[] ParsedArgs { get; }
        public object[] ParsedFlags { get; }
        public object[] CallArguments => ParsedArgs.Concat(ParsedFlags).ToArray();
        public (string message, Func<ITextResourceCollection, object>[] values) ErrorMessage { get; }

        private ArgumentCheckResponse(ArgumentCheckResult result, object[] args, object[] flags, (string, Func<ITextResourceCollection, object>[]) message)
        {
            SuccessStatus = result;
            ParsedArgs = args;
            ParsedFlags = flags;
            ErrorMessage = message;
        }

        public static ArgumentCheckResponse FromSuccess(object[] arguments, object[] flags)
            => new ArgumentCheckResponse(ArgumentCheckResult.Successful, arguments, flags, (null, null));
        public static ArgumentCheckResponse FromError(ArgumentCheckResult result, string message, Func<ITextResourceCollection, object>[] values)
            => new ArgumentCheckResponse(result, null, null, (message, values));
        public static ArgumentCheckResponse FromError(ArgumentCheckResult result, string message)
            => new ArgumentCheckResponse(result, null, null, (message, new Func<ITextResourceCollection, object>[0]));
    }

    enum ArgumentCheckResult
    {
        Successful,
        ArgumentMismatch,
        NotEnoughArguments,
        TooManyArguments,
        InvalidSubcall,
        Other
    }
}
