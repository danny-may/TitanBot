using System.Linq;

namespace TitanBotBase.Commands.Responses
{
    struct ArgumentCheckResponse
    {
        public ArgumentCheckResult SuccessStatus { get; }
        public object[] ParsedArgs { get; }
        public object[] ParsedFlags { get; }
        public object[] CallArguments => ParsedArgs.Concat(ParsedFlags).ToArray();
        public string ErrorMessage { get; }

        private ArgumentCheckResponse(ArgumentCheckResult result, object[] args, object[] flags, string message)
        {
            SuccessStatus = result;
            ParsedArgs = args;
            ParsedFlags = flags;
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
