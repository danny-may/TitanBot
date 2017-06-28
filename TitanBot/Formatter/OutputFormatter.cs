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

        protected IEnumerable<Type> KnownTypes => BeautifyDelegates.Keys;

        protected delegate string BeautifyDelegate<T>(T value);

        protected ICommandContext Context;
        protected bool AltFormat;
        public OutputFormatter(ICommandContext context, bool altFormat)
        {
            Context = context;
            AltFormat = altFormat;
        }

        public string Beautify<T>(T value)
        {
            if (KnownTypes.Contains(typeof(T)))
                return GetBeautify<T>()(value);
            else
                return value.ToString();
        }

        protected void Add<T>(BeautifyDelegate<T> beautify)
        {
            BeautifyDelegates[typeof(T)] = beautify;
        }

        protected BeautifyDelegate<T> GetBeautify<T>()
            => (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)];
    }
}
