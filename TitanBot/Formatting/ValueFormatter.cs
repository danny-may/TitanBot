using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBot.Commands;
using TitanBot.Util;

namespace TitanBot.Formatting
{
    public class ValueFormatter
    {
        private Dictionary<Type, Delegate> BeautifyDelegates { get; } = new Dictionary<Type, Delegate>();
        private MethodInfo BeautifyGeneric { get; }

        protected IEnumerable<Type> KnownTypes => BeautifyDelegates.Keys;

        protected delegate string BeautifyDelegate<T>(T value);

        protected ICommandContext Context;
        protected FormattingType AltFormat;
        public ValueFormatter(ICommandContext context)
        {
            Context = context;
            AltFormat = Context.UserSetting.FormatType;
            BeautifyGeneric = MiscUtil.GetMethod<ValueFormatter>(v => v.Beautify<object>(null));
        }

        public string Beautify<T>(T value)
        {
            if (typeof(T) == typeof(string))
                return value as string;
            if (KnownTypes.Contains(typeof(T)))
                return GetBeautify<T>()(value);
            else
                return value.ToString();
        }

        public string Beautify(object value)
            => (string)BeautifyGeneric.MakeGenericMethod(value.GetType())
                                      .Invoke(this, new object[] { value });

        protected void Add<T>(BeautifyDelegate<T> beautify)
            => BeautifyDelegates[typeof(T)] = beautify;

        protected BeautifyDelegate<T> GetBeautify<T>()
            => (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)];
    }
}
