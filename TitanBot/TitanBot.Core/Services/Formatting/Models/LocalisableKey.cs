using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class LocalisableKey : ILocalisable<string>
    {
        public string Key;
        private object[] _values;

        private LocalisableKey(string key, IEnumerable<object> values)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _values = values?.ToArray() ?? new object[0];
        }

        public static LocalisableKey From(string key, params object[] values)
            => new LocalisableKey(key, values);

        public static LocalisableKey From(string key, IEnumerable<object> values = null)
            => new LocalisableKey(key, values ?? new object[0]);

        public static LocalisableKey From<T>(string key, IEnumerable<T> values = null)
            => new LocalisableKey(key, (values ?? new T[0]).Cast<object>());

        object ILocalisable.Localise(ILocalisationCollection localeManager)
            => Localise(localeManager);

        public string Localise(ILocalisationCollection localeManager)
            => localeManager.Format(Key, _values.Select(v => v is ILocalisable l ? l.Localise(localeManager) : v));
    }
}