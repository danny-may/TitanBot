using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Commands;
using TitanBot.TypeReaders;

namespace TitanBot.Formatter
{
    public class OutputFormatter
    {
        private Dictionary<Type, Delegate> BeautifyDelegates { get; } = new Dictionary<Type, Delegate>();
        private Dictionary<Type, Delegate> TryParseDelegates { get; } = new Dictionary<Type, Delegate>();

        protected IEnumerable<Type> KnownTypes => BeautifyDelegates.Keys;

        protected delegate string BeautifyDelegate<T>(T value);
        protected delegate bool TryParseDelegate<T>(string text, out T value);

        protected ICommandContext Context;
        protected ITypeReaderCollection Readers;
        protected bool AltFormat;
        public OutputFormatter(ICommandContext context, ITypeReaderCollection typeReaders, bool altFormat)
        {
            Context = context;
            Readers = typeReaders;
            AltFormat = altFormat;
        }

        public string Beautify<T>(T value)
        {
            if (KnownTypes.Contains(typeof(T)))
                return GetBeautify<T>()(value);
            else
                return value.ToString();
        }

        public bool TryParse<T>(string text, out T value)
        {
            value = default(T);
            if (KnownTypes.Contains(typeof(T)))
                return GetTryParse<T>()(text, out value);
            else
                return false;
        }

        protected void Add<T>(BeautifyDelegate<T> beautify, TryParseDelegate<int> tryParse)
        {
            BeautifyDelegates[typeof(T)] = beautify;
            TryParseDelegates[typeof(T)] = tryParse;
        }

        protected BeautifyDelegate<T> GetBeautify<T>()
            => (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)];
        protected TryParseDelegate<T> GetTryParse<T>()
            => (TryParseDelegate<T>)TryParseDelegates[typeof(T)];
    }
}
