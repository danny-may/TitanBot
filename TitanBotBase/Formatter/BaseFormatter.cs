using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
