using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Formatting;

namespace TitanBot.TypeReaders
{
    class FormatingTypeTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(IMessageContext context, string value)
        {
            foreach (var format in context.Formatter.FormatNames)
                if (format.Name.ToUpper() == value.ToUpper())
                    return ValueTask.FromResult(TypeReaderResponse.FromSuccess(format.Format));

            return ValueTask.FromResult(TypeReaderResponse.FromError(TitanBotResource.TYPEREADER_UNABLETOREAD, value, typeof(FormattingType)));
        }
    }
}
