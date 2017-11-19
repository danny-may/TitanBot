using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titansmasher.Services.Displaying
{
    public class Translation : IDisplayable<string>
    {
        #region Fields

        public string Key => _key;
        public IReadOnlyList<object> Values => _values.ToImmutableArray();

        private readonly string _key;
        private readonly object[] _values;

        #endregion Fields

        #region Constructors

        public Translation(string key, params object[] values)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _values = values ?? new object[0];
        }

        public Translation(string key, IEnumerable<object> values)
            : this(key, values.ToArray())
        {
        }

        #endregion Constructors

        #region Overrides

        public override string ToString()
            => _key;

        #endregion Overrides

        #region IDisplayable

        object IDisplayable.Display(IDisplayService service, DisplayOptions options)
            => Display(service, options);

        public string Display(IDisplayService service, DisplayOptions options = default)
            => service.GetTranslation(_key, _values, options);

        #endregion IDisplayable
    }
}