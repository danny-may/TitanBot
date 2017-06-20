using System.Threading.Tasks;
using TitanBot.Util;

namespace TitanBot.Commands.DefautlCommands.General
{
    [Description("Basic command for calculating the delay of the bot.")]
    class PingCommand : Command
    {
        [Call]
        [Usage("Replies with a pong and what the current delay is.")]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync($"| ~{Client.Latency} ms", ReplyType.Success);
            await msg.ModifySafeAsync(m => m.Content = DiscordUtil.FormatMessage($"| {(msg.Timestamp - Message.Timestamp).TotalMilliseconds} ms", (int)ReplyType.Success));
        }
    }
}
