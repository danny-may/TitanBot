using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class TransText : IDisplayable<string>
    {
        #region Statics

        public static TransText From(string format, params object[] values)
            => new TransText(format, values);

        public static TransText From<T>(string format, params T[] values)
            => new TransText(format, values.Cast<object>());

        public static TransText From(string format, IEnumerable<object> values)
            => new TransText(format, values);

        public static TransText From<T>(string format, IEnumerable<T> values)
            => new TransText(format, values.Cast<object>());

        public static TransText Join(string separator, params object[] values)
            => Join(separator, values as IEnumerable<object>);

        public static TransText Join<T>(string separator, params T[] values)
            => Join(separator, values.Cast<object>());

        public static TransText Join(string separator, IEnumerable<object> values)
            => new TransText(string.Join(separator, values.Select((v, i) => $"{i}")), values);

        public static TransText Join<T>(string separator, IEnumerable<T> values)
            => Join(separator, values.Cast<object>());

        #endregion Statics

        #region Fields

        public readonly string Format;
        private readonly object[] _values;

        #endregion Fields

        #region Constructors

        private TransText(string format, IEnumerable<object> values)
        {
            Format = Format ?? throw new ArgumentNullException(nameof(format));
            _values = values?.ToArray() ?? new object[0];
        }

        #endregion Constructors

        #region IDisplayable

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        public string Display(ITranslationSet translations, IValueFormatter formatter)
            => string.Format(Format, _values.Select(v => formatter.Beautify(v is IDisplayable l ? l.Display(translations, formatter) : v)));

        #endregion IDisplayable
    }
}