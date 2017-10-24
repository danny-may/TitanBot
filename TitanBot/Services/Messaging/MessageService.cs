using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Messaging;

namespace TitanBot.Services.Messaging
{
    public class MessageService : IMessageService
    {
        private ITranslationService _transService;
        private IFormatterService _formatter;

        public MessageService(ITranslationService transService, IFormatterService formatter)
        {
            _transService = transService;
            _formatter = formatter;
        }

        private ISendContext GetSend(IMessageChannel channel, IUser user)
            => new SendContext(channel, user, _transService, _formatter);

        private IEditContext GetEdit(IUserMessage message, IUser user)
            => new EditContext(message, user, _transService, _formatter);

        public ISendContext Send(IMessageChannel channel, IUser user)
            => GetSend(channel, user);

        public IUserMessage Send(IMessageChannel channel, IUser user, Action<ISendContext> action, RequestOptions options = null)
            => action.RunInline(GetSend(channel, user)).Send(options);

        public Task<IUserMessage> SendAsync(IMessageChannel channel, IUser user, Action<ISendContext> action, RequestOptions options = null)
            => action.RunInline(GetSend(channel, user)).SendAsync(options);

        public IEditContext Edit(IUserMessage message, IUser user)
            => GetEdit(message, user);

        public void Edit(IUserMessage message, IUser user, Action<IEditContext> action, RequestOptions options = null)
            => action.RunInline(GetEdit(message, user)).Edit(options);

        public Task EditAsync(IUserMessage message, IUser user, Action<IEditContext> action, RequestOptions options = null)
            => action.RunInline(GetEdit(message, user)).EditAsync(options);
    }
}