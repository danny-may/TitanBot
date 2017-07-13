using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Logging;
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
            return await channel.SendMessageSafeAsync(message, async e => {
                await (await user.GetOrCreateDMChannelAsync()).SendMessageSafeAsync("I was unable to reply to you! This is the message I tried to send:\n" + message, embed: embed);
                await Logger.LogAsync(e, $"Command: {GetType().Name}");
                await (handler?.Invoke(e) ?? Task.CompletedTask);
            }, isTTS, embed, options);
        }

        public IUserMessage Reply(IMessageChannel channel, IUser user, string message, ReplyType replyType = ReplyType.None, Func<Exception, Task> handler = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => ReplyAsync(channel, user, message, replyType, handler, isTTS, embed, options).Result;
    }
}
