using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titansmasher.Services.Displaying
{
    public class TextLiteral : IDisplayable<string>
    {
        #region Fields

        public string Format => _format;
        public IReadOnlyList<object> Values => _values.ToImmutableArray();

        private readonly string _format;
        private readonly object[] _values;

        #endregion Fields

        #region Constructors

        public TextLiteral(string format, params object[] values)
        {
            _format = format ?? throw new System.ArgumentNullException(nameof(format));
            _values = values ?? new object[0];
        }

        public TextLiteral(string format, IEnumerable<object> values)
            : this(format, values.ToArray())
        {
        }

        #endregion Constructors

        #region Overrides

        public override string ToString()
            => _format;

        #endregion Overrides

        #region IDisplayable

        object IDisplayable.Display(IDisplayService service, DisplayOptions options)
            => Display(service, options);

        public string Display(IDisplayService service, DisplayOptions options = default)
            => string.Format(_format, _values.Select(v => v is IDisplayable d ? d.Display(service, options) : v));

        #endregion IDisplayable
    }
}