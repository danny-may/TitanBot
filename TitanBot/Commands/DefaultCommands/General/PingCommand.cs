using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("Basic command for calculating the delay of the bot.")]
    public class PingCommand : Command
    {
        [Call]
        [Usage("Replies with a pong and what the current delay is.")]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync(TextResource.Format("PING_INITIAL", Client.Latency), ReplyType.Success);
            await msg.ModifySafeAsync(m => m.Content = DiscordUtil.FormatMessage(TextResource.Format("PING_VERIFY", (msg.Timestamp - Message.Timestamp).TotalMilliseconds), (int)ReplyType.Success));
        }
    }
}
