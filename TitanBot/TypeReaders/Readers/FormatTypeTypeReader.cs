using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Formatting;
using static TitanBot.TBLocalisation.Logic;

namespace TitanBot.TypeReaders
{
    class FormatTypeTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(IMessageContext context, string value)
        {
            foreach (var format in context.Formatter.KnownFormats)
                if (format.Localise(context.TextResource).ToUpper() == value.ToUpper())
                    return ValueTask.FromResult(TypeReaderResponse.FromSuccess(format));

            return ValueTask.FromResult(TypeReaderResponse.FromError(TYPEREADER_UNABLETOREAD, value, typeof(FormatType)));
        }
    }
}
