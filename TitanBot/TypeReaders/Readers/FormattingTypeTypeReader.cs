using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Formatting;

namespace TitanBot.TypeReaders
{
    class FormatingTypeTypeReader : TypeReader
    {
        public override Task<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            foreach (var format in context.Formatter.FormatNames)
                if (format.Name.ToUpper() == value.ToUpper())
                    return Task.FromResult(TypeReaderResponse.FromSuccess(format.Format));

            return Task.FromResult(TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", value, typeof(FormattingType)));
        }
    }
}
