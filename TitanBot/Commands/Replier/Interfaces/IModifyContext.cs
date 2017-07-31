using Discord;
using System;
using System.Threading.Tasks;

namespace TitanBot.Commands
{

    public delegate Task OnModifyEventHandler(IModifyContext context, IUserMessage message);
    public delegate Task MessageModifyErrorHandler(Exception ex, IUserMessage message, IMessageContext context, string text, IEmbedable embed);

    public interface IModifyContext
    {
        event OnModifyEventHandler OnModify;

        IModifyContext ChangeMessage(string message);
        IModifyContext ChangeMessage(string message, ReplyType replyType);
        IModifyContext ChangeMessage(string message, params object[] values);
        IModifyContext ChangeMessage(string message, ReplyType replyType, params object[] values);
        IModifyContext ChangeEmbedable(IEmbedable embedable);
        IModifyContext WithRequestOptions(RequestOptions options);
        IModifyContext WithErrorHandler(MessageModifyErrorHandler handler);

        Task ModifyAsync(bool stealthy = false);
        void Modify(bool stealthy = false);
    }
}
