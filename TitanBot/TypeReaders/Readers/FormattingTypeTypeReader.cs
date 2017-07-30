using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Formatting;

namespace TitanBot.TypeReaders
{
    class FormatingTypeTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            foreach (var format in context.Formatter.FormatNames)
                if (format.Name.ToUpper() == value.ToUpper())
                    return ValueTask.FromResult(TypeReaderResponse.FromSuccess(format.Format));

            return ValueTask.FromResult(TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", value, typeof(FormattingType)));
        }
    }
}
