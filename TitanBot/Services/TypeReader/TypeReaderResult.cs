using System.Linq;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Core.Services.TypeReader
{
    public sealed class TypeReaderResult : ITypeReaderResult
    {
        public bool IsSuccess { get; private set; }
        public ITypeReaderMatch[] Matches { get; private set; }
        public ITypeReaderMatch BestMatch => Matches.OrderByDescending(m => m.Certainty).FirstOrDefault();
        public IDisplayable<string> Message { get; private set; }

        private TypeReaderResult()
        {
        }

        public static TypeReaderResult FromSuccess(string initialText, object value)
            => FromSuccess(initialText, new TypeReaderMatch(100, value));

        public static TypeReaderResult FromSuccess(string initialText, params ITypeReaderMatch[] matches)
            => new TypeReaderResult
            {
                IsSuccess = true,
                Matches = matches,
                Message = TransText.From($"{initialText} matched successfully")
            };

        public static TypeReaderResult FromError(IDisplayable<string> message)
            => new TypeReaderResult
            {
                IsSuccess = false,
                Matches = new TypeReaderMatch[0],
                Message = message
            };

        public override string ToString()
            => IsSuccess ? $"Matches: {Matches.Length} | Best: {{{BestMatch}}}" : $"Unsuccessful: {Message}";
    }
}