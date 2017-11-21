using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Titansmasher.Services.Display.Interfaces;

namespace Titansmasher.Services.Display
{
    public class TextLiteral : IDisplayable<string>
    {
        #region Statics

        public static TextLiteral Join(string separator, IEnumerable<object> values)
            => new TextLiteral(string.Join(separator, values.Select((v, i) => $"{{{i}}}")), values);

        #endregion Statics

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

        #region Operators

        public static implicit operator TextLiteral(string text)
            => new TextLiteral(text);

        #endregion Operators

        #region IDisplayable

        object IDisplayable.Display(IDisplayService service, DisplayOptions options)
            => Display(service, options);

        public string Display(IDisplayService service, DisplayOptions options = default)
            => string.Format(_format, service.Beautify(_values as IEnumerable<object>));

        #endregion IDisplayable
    }
}