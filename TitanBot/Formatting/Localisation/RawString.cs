using System.Linq;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Formatting
{
    public class RawString : LocalisedString
    {
        public RawString(string text) : base(text) { }
        public RawString(string text, params object[] values) : base(text, values) { }

        public override string Localise(ITextResourceCollection textResource)
        {
            if (Key == null) return null;
            var prepped = Values.Select(v => v is ILocalisable ls ? ls.Localise(textResource) : v).ToArray();
            return string.Format(Key, prepped);
        }

        public static implicit operator RawString(string text)
            => new RawString(text);
        public static implicit operator RawString((string Text, object Value1) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1 });
        public static implicit operator RawString((string Text, object Value1, object Value2) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2 });
        public static implicit operator RawString((string Text, object Value1, object Value2, object Value3) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3 });
        public static implicit operator RawString((string Text, object Value1, object Value2, object Value3, object Value4) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4 });
        public static implicit operator RawString((string Text, object Value1, object Value2, object Value3, object Value4, object Value5) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5 });
        public static implicit operator RawString((string Text, object Value1, object Value2, object, object Value3, object Value4, object Value5, object Value6) tuple)
            => new RawString(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5, tuple.Value6 });
    }
}
