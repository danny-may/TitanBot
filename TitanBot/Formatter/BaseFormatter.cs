using TitanBot.Commands;
using TitanBot.TypeReaders;

namespace TitanBot.Formatter
{
    class BaseFormatter : OutputFormatter
    {
        public BaseFormatter(ICommandContext context, ITypeReaderCollection typeReaders, bool altFormat) : base(context, typeReaders, altFormat)
        {
        }
    }
}
