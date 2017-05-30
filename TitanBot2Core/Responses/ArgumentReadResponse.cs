using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Responses
{
    public struct ArgumentReadResponse
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public Reason ErrorReason { get; private set; }
        public object[] ParsedArguments { get; private set; }
        public double Weight => ParsedArguments.Sum(a => Weigh(a?.GetType()));

        public static ArgumentReadResponse FromSuccess(TypeReaderResponse[] args)
        {
            if (args.Any(a => !a.IsSuccess))
                return FromError(Reason.IncorrectArgumentType);
            else
                return new ArgumentReadResponse
                {
                    IsSuccess = true,
                    ParsedArguments = args.Select(a => a.Best).ToArray()
                };
        }

        public static ArgumentReadResponse FromError(Reason reason, string message = null)
            => new ArgumentReadResponse
            {
                IsSuccess = false,
                ErrorReason = reason,
                Message = message
            };

        private static double Weigh(Type type)
            => FluentSwitcher.Switch<Type, double>(type)
                    .Case(typeof(string), () => 0.1)
                    .Default(() => 1);

        public enum Reason
        {
            NotEnoughArguments,
            IncorrectArgumentType,
            TooManyArguments
        }
    }
}
