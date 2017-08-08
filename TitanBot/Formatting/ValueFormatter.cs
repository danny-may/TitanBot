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
        private Dictionary<Type, Dictionary<FormatType, Delegate>> BeautifyDelegates { get; } = new Dictionary<Type, Dictionary<FormatType, Delegate>>();
        private MethodInfo BeautifyGeneric { get; }
        private HashSet<FormatType> Formats { get; } = new HashSet<FormatType> { FormatType.DEFAULT };

        protected delegate string BeautifyDelegate<T>(T value);

        public Type[] KnownTypes => BeautifyDelegates.Keys.ToArray();
        public FormatType[] KnownFormats => Formats.ToArray();

        public ValueFormatter()
        {
            BeautifyGeneric = MiscUtil.GetMethod<ValueFormatter>(v => v.Beautify<object>(FormatType.DEFAULT, null));
        }

        public string Beautify<T>(FormatType format, T value)
        {
            if (typeof(T) == typeof(string))
                return value as string;
            if (KnownTypes.Contains(typeof(T)))
                return GetBeautify<T>(format)(value);
            else
                return value.ToString();
        }

        public string Beautify(FormatType format, object value)
        {
            if (value == null)
                return null;
            return (string)BeautifyGeneric.MakeGenericMethod(value.GetType()).Invoke(this, new object[] { format, value });
        }

        public string Beautify(FormatType format, Type type, object value)
        {
            if (value == null || type == null)
                return null;
            return (string)BeautifyGeneric.MakeGenericMethod(type).Invoke(this, new object[] { format, value });
        }

        protected void Register(FormatType format)
            => Formats.Add(format);

        protected void Add<T>(BeautifyDelegate<T> beautify)
            => Add(beautify, new (FormatType, BeautifyDelegate<T>)[0]);
        protected void Add<T>(BeautifyDelegate<T> beautify, params (FormatType Format, BeautifyDelegate<T> Delegate)[] others)
        {
            if (others?.Any(t => !Formats.Contains(t.Format)) ?? false)
                throw new ArgumentException("You must register a format before supplying a conversion for it", nameof(others));
            var type = typeof(T);
            if (!BeautifyDelegates.ContainsKey(type))
                BeautifyDelegates[type] = new Dictionary<FormatType, Delegate>();
            BeautifyDelegates[type][FormatType.DEFAULT] = beautify;
            foreach (var b in others)
                BeautifyDelegates[type][b.Format] = b.Delegate;
        }

        protected BeautifyDelegate<T> GetBeautify<T>(FormatType format)
        {
            if (BeautifyDelegates[typeof(T)].TryGetValue(format, out Delegate beautifier))
                return (BeautifyDelegate<T>)beautifier;
            return (BeautifyDelegate<T>)BeautifyDelegates[typeof(T)][FormatType.DEFAULT];
        }
    }
}
