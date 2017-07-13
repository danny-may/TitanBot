using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace TitanBot.TextResource
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
