using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Services.Messaging
{
    public interface IMessageService
    {
        ISendContext Send(IMessageChannel channel, IUser user);
        IUserMessage Send(IMessageChannel channel, IUser user, Action<ISendContext> action, RequestOptions options = null);
        Task<IUserMessage> SendAsync(IMessageChannel channel, IUser user, Action<ISendContext> action, RequestOptions options = null);

        IEditContext Edit(IUserMessage message, IUser user);
        void Edit(IUserMessage message, IUser user, Action<IEditContext> action, RequestOptions options = null);
        Task EditAsync(IUserMessage message, IUser user, Action<IEditContext> action, RequestOptions options = null);
    }
}