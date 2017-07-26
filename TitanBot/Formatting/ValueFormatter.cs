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

        protected HashSet<(FormattingType Format, string Name)> Names { get; } = new HashSet<(FormattingType Format, string Name)> { (FormattingType.DEFAULT, "Default") };
        public IReadOnlyCollection<(FormattingType Format, string Name)> FormatNames => Names.ToList().AsReadOnly();

        protected IEnumerable<Type> KnownTypes => BeautifyDelegates.Keys;
        protected delegate string BeautifyDelegate<T>(T value);        
        protected ICommandContext Context;
        protected FormattingType AltFormat = FormattingType.DEFAULT;
        public FormattingType[] AcceptedFormats => Names.Select(n => n.Format).ToArray();

        public virtual string GetName(FormattingType format)
            => Names.FirstOrDefault(n => n.Format == format).Name ?? "UNKNOWN";

        public ValueFormatter()
        {
            BeautifyGeneric = MiscUtil.GetMethod<ValueFormatter>(v => v.Beautify<object>(null));
        }

        public ValueFormatter(ICommandContext context) : this()
        {
            Context = context;
            AltFormat = Context.GeneralUserSetting.FormatType;
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
        {
            if (value == null)
                return null;
            return (string)BeautifyGeneric.MakeGenericMethod(value.GetType())
                                      .Invoke(this, new object[] { value });
        }

        protected void Add<T>(BeautifyDelegate<T> beautify)
            => BeautifyDelegates[typeof(T)] = beautify;

        protected BeautifyDelegate<T> GetBeautify<T>()
            => (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)];
    }
}
