using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Commands;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Replying
{

    public delegate Task OnModifyEventHandler(IModifyContext context, IUserMessage message);
    public delegate Task MessageModifyErrorHandler(Exception ex, IUserMessage message, string text, IEmbedable embed);

    public interface IModifyContext
    {
        event OnModifyEventHandler OnModify;

        IModifyContext ChangeMessage(ILocalisable<string> message);
        IModifyContext ChangeRawMessage(string message);
        IModifyContext ChangeMessage(string message);
        IModifyContext ChangeMessage(string message, ReplyType replyType);
        IModifyContext ChangeMessage(string message, params object[] values);
        IModifyContext ChangeMessage(string message, ReplyType replyType, params object[] values);
        IModifyContext ChangeEmbedable(IEmbedable embedable);
        IModifyContext ChangeEmbedable(ILocalisable<EmbedBuilder> embedable);
        IModifyContext WithRequestOptions(RequestOptions options);
        IModifyContext WithErrorHandler(MessageModifyErrorHandler handler);

        Task ModifyAsync(bool stealthy = false);
        void Modify(bool stealthy = false);
    }
}
