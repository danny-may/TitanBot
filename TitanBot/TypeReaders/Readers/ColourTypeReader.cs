using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TypeReaders
{
    class ColourTypeReader : TypeReader
    {
        public override ValueTask<TypeReaderResponse> Read(ICommandContext context, string value)
        {
            var colour = Color.FromName(value);
            if (colour.IsKnownColor)
                return ValueTask.FromResult(TypeReaderResponse.FromSuccess(colour));

            var input = (string)value.Clone();
            if (value.StartsWith("#"))
                input = value.Substring(1);

            int r = 0;
            int g = 0;
            int b = 0;

            int charsPerVal;

            if (input.Length == 3)
                charsPerVal = 1;
            else if (input.Length == 6)
                charsPerVal = 2;
            else
                return ValueTask.FromResult(TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", value, typeof(Color)));

            if (!int.TryParse(input.Substring(0, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out r) ||
                !int.TryParse(input.Substring(charsPerVal, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out g) ||
                !int.TryParse(input.Substring(2 * charsPerVal, charsPerVal), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out b))
                return ValueTask.FromResult(TypeReaderResponse.FromError("TYPEREADER_UNABLETOREAD", value, typeof(Color)));

            return ValueTask.FromResult(TypeReaderResponse.FromSuccess(Color.FromArgb(r, g, b)));

        }
    }
}
