using TitanBotBase.Commands;
using TitanBotBase.TypeReaders;

namespace TitanBotBase.Formatter
{
    class BaseFormatter : OutputFormatter
    {
        public BaseFormatter(ICommandContext context, ITypeReaderCollection typeReaders, bool altFormat) : base(context, typeReaders, altFormat)
        {
        }
    }
}
