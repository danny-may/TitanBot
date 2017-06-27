using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Logger;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public class Replier : IReplier
    {
        private ILogger Logger { get; }

        public Replier(ILogger logger)
        {
            Logger = logger;
        }

        public async Task<IUserMessage> ReplyAsync(IMessageChannel channel, IUser user, string message, ReplyType replyType = ReplyType.None, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            var formattedMessage = DiscordUtil.FormatMessage(message, (int)replyType);
            return await channel.SendMessageSafeAsync(formattedMessage, async e => {
                await (await user.GetOrCreateDMChannelAsync()).SendMessageSafeAsync(DiscordUtil.FormatMessage("I was unable to reply to you! This is the message I tried to send:\n" + formattedMessage, 1));
                await Logger.LogAsync(e, $"Command: {GetType().Name}");
                await (handler?.Invoke(e) ?? Task.CompletedTask);
            }, isTTS, embed, options);
        }

        public IUserMessage Reply(IMessageChannel channel, IUser user, string message, ReplyType replyType = ReplyType.None, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(channel, user, message, replyType, handler, isTTS, embed, options).Result;
    }
}
