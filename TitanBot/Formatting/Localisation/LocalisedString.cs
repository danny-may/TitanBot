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
        public static LocalisedString JoinEnumerable<T>(string separator, IEnumerable<T> values)
            => Join(separator, values.Cast<object>().ToArray());

        public static explicit operator LocalisedString(string text)
            => Build(text);
    }
}
