using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Database;
using TitanBotBase.TypeReaders;

namespace TitanBotBase.Formatter
{
    public abstract class OutputFormatter
    {
        protected ICommandContext Context;
        protected ITypeReaderCollection Readers;
        protected bool AltFormat;
        public OutputFormatter(ICommandContext context, ITypeReaderCollection typeReaders, bool altFormat)
        {
            Context = context;
            Readers = typeReaders;
            AltFormat = altFormat;
        }

        public virtual string Beautify<T>(T value)
        {
            return value.ToString();
        }

        public virtual bool TryParse<T>(string text, out T value)
        {
            value = default(T);
            var readerResult = Readers.Read(typeof(T), Context, text).Result;
            if (readerResult.IsSuccess)
                value = (T)readerResult.Best;
            return readerResult.IsSuccess;
        }

    }
}
