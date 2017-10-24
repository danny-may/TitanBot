using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Services.Formatting
{
    public class ValueFormatter : IValueFormatter
    {
        #region Fields

        private readonly ConcurrentDictionary<Type, Delegate> _formatters = new ConcurrentDictionary<Type, Delegate>();
        private readonly MethodInfo BeautifyGeneric;

        #endregion Fields

        #region Constructors

        public ValueFormatter(FormatType formatType)
        {
            FormatType = formatType;
            BeautifyGeneric = ReflectionExtensions.GetMethod<ValueFormatter>(v => v.Beautify<object>(null));
        }

        #endregion Constructors

        #region Methods

        internal void AddFormatter<T>(BeautifyDelegate<T> formatter)
            => _formatters.AddOrUpdate(typeof(T), formatter, (t, x) => formatter);

        #endregion Methods

        #region IValueFormatter

        public FormatType FormatType { get; }

        public Type[] KnownTypes => _formatters.Keys.ToArray();

        public string Beautify<T>(T value)
            => _formatters.TryGetValue(typeof(T), out var formatter) ? (formatter as BeautifyDelegate<T>)(value) : value.ToString();

        public string Beautify(object value)
            => Beautify(value?.GetType(), value);

        public string Beautify(Type type, object value)
            => (type == null || value == null) ? null : (string)BeautifyGeneric.MakeGenericMethod(type).Invoke(this, new[] { value });

        #endregion IValueFormatter
    }
}