using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBot.Settings;
using TitanBot.Util;

namespace TitanBot.Formatting
{
    public class ValueFormatter
    {
        private Dictionary<Type, Dictionary<FormattingType, Delegate>> BeautifyDelegates { get; } = new Dictionary<Type, Dictionary<FormattingType, Delegate>>();
        private MethodInfo BeautifyGeneric { get; }

        protected HashSet<(FormattingType Format, string Name)> Names { get; } = new HashSet<(FormattingType Format, string Name)> { (FormattingType.DEFAULT, "Default") };
        public IReadOnlyCollection<(FormattingType Format, string Name)> FormatNames => Names.ToList().AsReadOnly();

        protected IEnumerable<Type> KnownTypes => BeautifyDelegates.Keys;
        protected delegate string BeautifyDelegate<T>(T value);
        public FormattingType[] AcceptedFormats => Names.Select(n => n.Format).ToArray();

        public virtual string GetName(FormattingType format)
            => Names.FirstOrDefault(n => n.Format == format).Name ?? "UNKNOWN";

        public ValueFormatter()
        {
            BeautifyGeneric = MiscUtil.GetMethod<ValueFormatter>(v => v.Beautify<object>(FormattingType.DEFAULT, null));
        }

        public string Beautify<T>(FormattingType format, T value)
        {
            if (typeof(T) == typeof(string))
                return value as string;
            if (KnownTypes.Contains(typeof(T)))
                return GetBeautify<T>(format)(value);
            else
                return value.ToString();
        }

        public string Beautify(FormattingType format, object value)
        {
            if (value == null)
                return null;
            return (string)BeautifyGeneric.MakeGenericMethod(value.GetType())
                                      .Invoke(this, new object[] { format, value });
        }

        protected void Add<T>(BeautifyDelegate<T> beautify, params (FormattingType, BeautifyDelegate<T>)[] others)
        {
            var type = typeof(T);
            if (!BeautifyDelegates.ContainsKey(type))
                BeautifyDelegates[type] = new Dictionary<FormattingType, Delegate>();
            BeautifyDelegates[type][FormattingType.DEFAULT] = beautify;
            foreach (var b in others)
                BeautifyDelegates[type][b.Item1] = b.Item2;
        }

        protected BeautifyDelegate<T> GetBeautify<T>(FormattingType format)
        {
            if (BeautifyDelegates[typeof(T)].TryGetValue(format, out Delegate beautifier))
                return (BeautifyDelegate<T>)beautifier;
            return (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)][FormattingType.DEFAULT];
        }
    }
}
