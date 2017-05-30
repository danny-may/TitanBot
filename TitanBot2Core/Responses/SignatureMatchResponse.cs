using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Flags;
using TitanBot2.TypeReaders;

namespace TitanBot2.Responses
{
    public struct SignatureMatchResponse
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public ArgumentReadResponse[] Arguments { get; private set; }
        public FlagCollection Flags { get; private set; }
        public double Weight => Arguments.Sum(a => a.Weight);

        public static SignatureMatchResponse FromSuccess(ArgumentReadResponse[] args, FlagCollection flags)
            => new SignatureMatchResponse
            {
                IsSuccess = true,
                Arguments = args.OrderByDescending(a => a.Weight).ToArray(),
                Flags = flags
            };

        public static SignatureMatchResponse FromError(string message = null)
            => new SignatureMatchResponse
            {
                IsSuccess = false,
                Message = message
            };
    }
}
