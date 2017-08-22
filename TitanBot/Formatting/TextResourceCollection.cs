using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Replying;

namespace TitanBot.Formatting
{
    class TextResourceCollection : ITextResourceCollection
    {
        private Dictionary<string, string> Values { get; }
        public ValueFormatter Formatter { get; }
        public FormatType FormatType { get; }
        public double Coverage { get; }

        public string this[string key] => GetResource(key);

        public TextResourceCollection(double coverage, ValueFormatter formatter, FormatType format, Dictionary<string, string> values)
        {
            Coverage = coverage;
            Values = values;
            Formatter = formatter;
            FormatType = format;
        }

        public string GetResource(string key)
        {
            if (key == null)
                return null;
            if (key.Contains(' '))
                return key;
            if (!Values.ContainsKey(key.ToUpper()))
                return key;
            var val = Values[key.ToUpper()];
            return val ?? key;
        }

        public string Format(string key, params object[] items)
        {
            try
            {
                return string.Format(GetResource(key), items.Select(i => Formatter.Beautify(FormatType, i)).ToArray());
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Key \"{key}\" ({GetResource(key)}) accepts more values than supplied ({items.Length})", ex);
            }
        }

        public string Format(string key, ReplyType replyType, params object[] items)
            => GetReplyType(replyType) + Format(key, items);

        public string GetResource(string key, ReplyType replyType)
            => GetReplyType(replyType) + GetResource(key);

        public string GetReplyType(ReplyType replyType)
        {
            switch (replyType)
            {
                case ReplyType.Success:
                    return GetResource(TBLocalisation.REPLYTYPE_SUCCESS);
                case ReplyType.Error:
                    return GetResource(TBLocalisation.REPLYTYPE_ERROR);
                case ReplyType.Info:
                    return GetResource(TBLocalisation.REPLYTYPE_INFO);
                default:
                    return "";
            }
        }
    }
}
