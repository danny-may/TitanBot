using TitanBot.Core.Models.Contexts;

namespace TitanBot.Core.Services.TypeReader
{
    public interface ITypeReader
    {
        ITypeReaderResult Read(IMessageContext context, string text);
    }
}