using TitanBot.Commands;

namespace TitanBot.Formatting
{
    public interface ITextResourceCollection
    {
        string Format(string key, params object[] items);
        string Format(string key, ReplyType replyType, params object[] items);
        string GetResource(string key);
        string GetResource(string key, ReplyType replyType);
        string GetReplyType(ReplyType replyType);
        string this[string index] { get; }
        double Coverage { get; }
    }
}
