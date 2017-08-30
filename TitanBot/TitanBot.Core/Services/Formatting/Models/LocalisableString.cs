using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class LocalisableString : ILocalisable<string>
    {
        public string Format { get; }
        private object[] _values { get; }

        private LocalisableString(string format, IEnumerable<object> values)
        {
            Format = Format ?? throw new ArgumentNullException(nameof(format));
            _values = values?.ToArray() ?? new object[0];
        }

        public static LocalisableString From(string format, params object[] values)
            => new LocalisableString(format, values);

        public static LocalisableString From<T>(string format, params T[] values)
            => new LocalisableString(format, values.Cast<object>());

        public static LocalisableString From(string format, IEnumerable<object> values)
            => new LocalisableString(format, values);

        public static LocalisableString From<T>(string format, IEnumerable<T> values)
            => new LocalisableString(format, values.Cast<object>());

        public static LocalisableString Join(string separator, params object[] values)
            => Join(separator, values as IEnumerable<object>);

        public static LocalisableString Join<T>(string separator, params T[] values)
            => Join(separator, values.Cast<object>());

        public static LocalisableString Join(string separator, IEnumerable<object> values)
            => new LocalisableString(string.Join(separator, values.Select((v, i) => $"{i}")), values);

        public static LocalisableString Join<T>(string separator, IEnumerable<T> values)
            => Join(separator, values.Cast<object>());

        object ILocalisable.Localise(ILocalisationCollection localeManager)
            => Localise(localeManager);

        public string Localise(ILocalisationCollection localeManager)
            => string.Format(Format, _values.Select(v => v is ILocalisable l ? l.Localise(localeManager) : v));
    }
}