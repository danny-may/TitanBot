using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Formatting;

namespace TitanBot.TypeReaders
{
    class LocaleTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(IMessageContext context, string value)
        {
            if (context.TextManager.GetLanguageCoverage(value) > 0)
                return ValueTask.FromResult(TypeReaderResponse.FromSuccess((Locale)value));
            return ValueTask.FromResult(TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_UNABLETOREAD, value, typeof(Locale)));
        }
    }
}
