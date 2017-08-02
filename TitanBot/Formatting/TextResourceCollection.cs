using System.Collections.Generic;
using System.Linq;
using TitanBot.Replying;

namespace TitanBot.Formatting
{
    class TextResourceCollection : ITextResourceCollection
    {
        private Dictionary<string, (string defaultText, string langText)> Values { get; }
        private ValueFormatter Formatter { get; }
        private FormattingType FormatType { get; }
        public double Coverage { get; }

        public string this[string key] => GetResource(key);

        public TextResourceCollection(double coverage, ValueFormatter formatter, FormattingType format, Dictionary<string, (string defaultText, string langText)> values)
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
            if (key.StartsWith("_"))
                return key.Substring(1);
            if (!Values.ContainsKey(key.ToUpper()))
                return key;
            var val = Values[key.ToUpper()];
            return val.langText ?? val.defaultText ?? key;
        }

        public string Format(string key, params object[] items)
            => string.Format(GetResource(key), items.Select(i => Formatter.Beautify(FormatType, i)).ToArray());

        public string Format(string key, ReplyType replyType, params object[] items)
            => GetReplyType(replyType) + Format(key, items);

        public string GetResource(string key, ReplyType replyType)
            => GetReplyType(replyType) + GetResource(key);

        public string GetReplyType(ReplyType replyType)
        {
            switch (replyType)
            {
                case ReplyType.Success:
                    return GetResource(TitanBotResource.REPLYTYPE_SUCCESS);
                case ReplyType.Error:
                    return GetResource(TitanBotResource.REPLYTYPE_ERROR);
                case ReplyType.Info:
                    return GetResource(TitanBotResource.REPLYTYPE_INFO);
                default:
                    return "";
            }
        }
    }
}
