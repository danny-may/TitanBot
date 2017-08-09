using System.Linq;
using TitanBot.Formatting.Interfaces;
using TitanBot.Replying;

namespace TitanBot.Formatting
{
    public class RawString : LocalisedString
    {
        public RawString(string text) : base(text) { }
        public RawString(string text, params object[] values) : base(text, values) { }
        public RawString(string text, ReplyType replyType) : base(text, replyType) { }
        public RawString(string text, ReplyType replyType, params object[] values) : base(text, replyType, values) { }

        public override string Localise(ITextResourceCollection textResource)
        {
            if (Key == null) return null;
            var prepped = Values.Select(v => v is ILocalisable ls ? ls.Localise(textResource) : v).ToArray();
            return textResource.GetReplyType(ReplyType) + string.Format(Key, prepped);
        }

        public static implicit operator RawString(string text)
            => text == null ? null : new RawString(text);
    }
}
