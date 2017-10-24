using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Services.TypeReader
{
    public interface ITypeReaderResult
    {
        ITypeReaderMatch BestMatch { get; }
        ITypeReaderMatch[] Matches { get; }
        IDisplayable<string> Message { get; }
        bool IsSuccess { get; }
    }
}