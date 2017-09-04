using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Core.Services.Formatting.Models
{
    public class TransKey : IDisplayable<string>
    {
        #region Statics

        public static TransKey From(string key, params object[] values)
            => new TransKey(key, values);

        public static TransKey From(string key, IEnumerable<object> values = null)
            => new TransKey(key, values ?? new object[0]);

        public static TransKey From<T>(string key, IEnumerable<T> values = null)
            => new TransKey(key, (values ?? new T[0]).Cast<object>());

        #endregion Statics

        #region Fields

        public readonly string Key;
        private readonly object[] _values;

        #endregion Fields

        #region Constructors

        private TransKey(string key, IEnumerable<object> values)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _values = values?.ToArray() ?? new object[0];
        }

        #endregion Constructors

        #region IDisplayable

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        public string Display(ITranslationSet translations, IValueFormatter formatter)
            => translations.GetTranslation(Key, _values.Select(v => formatter.Beautify(v is IDisplayable<string> l ? l.Display(translations, formatter) : v)).ToArray());

        #endregion IDisplayable
    }
}