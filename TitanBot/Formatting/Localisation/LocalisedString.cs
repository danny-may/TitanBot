using System.Collections.Generic;
using System.Linq;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Formatting.Interfaces;
using System;

namespace TitanBot.Formatting
{
    public class LocalisedString : ILocalisable<string>
    {
        public string Key { get; }
        public ReplyType ReplyType { get; } = ReplyType.None;
        public object[] Values => _values.ToArray();

        private List<object> _values { get; } = new List<object>();

        public LocalisedString(object value) : this("{0}", ReplyType.None, new object[] { value }) { }
        public LocalisedString(string text) : this(text, ReplyType.None, values: null) { }
        public LocalisedString(string key, ReplyType replyType) : this(key, replyType, values: null) { }
        public LocalisedString(string key, params object[] values) : this(key, ReplyType.None, values) { }
        public LocalisedString(string key, ReplyType replyType, params object[] values)
        {
            Key = key;
            ReplyType = replyType;
            _values.AddRange(values ?? new object[0]);
        }

        public LocalisedString AddValue(object value)
        {
            _values.Add(value);
            return this;
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public virtual string Localise(ITextResourceCollection textResource)
        {
            if (Key == null) return null;
            var prepped = _values.Select(v => v is ILocalisable ls ? ls.Localise(textResource) : v).ToArray();
            return textResource.Format(Key, ReplyType, prepped);
        }

        public override string ToString()
            => Key;
        
        private static LocalisedString Build(string text, ReplyType replyType = ReplyType.None, object[] values = null)
        {
            if (text == null) return null;            

            return new LocalisedString(text, replyType, values ?? new object[0]);
        }

        public static LocalisedString Join(string separator, params object[] values)
            => new LocalisedString(string.Join(separator, values.Select((v, i) => $"{{{i}}}")), values);

        public static explicit operator LocalisedString(string text)
            => Build(text);
        public static implicit operator LocalisedString((string Text, object Value1) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3, object Value4) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3, object Value4, object Value5) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object, object Value3, object Value4, object Value5, object Value6) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5, tuple.Value6});
        public static implicit operator LocalisedString((string Key, ReplyType ReplyType) tuple)
            => Build(tuple.Key, tuple.ReplyType);
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3, object Value4) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3, object Value4, object Value5) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object, object Value3, object Value4, object Value5, object Value6) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5, tuple.Value6 });
        public static implicit operator LocalisedString((string Key, object[] Values) tuple)
            => Build(tuple.Key, values: tuple.Values);
        public static implicit operator LocalisedString((string Key, ReplyType ReplyType, object[] Values) tuple)
            => Build(tuple.Key, tuple.ReplyType, tuple.Values);
    }
}
