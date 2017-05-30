using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.General
{
    [Description("Basic command for calculating the delay of the bot.")]
    class PingCommand : Command
    {
        [Call]
        [Usage("Replies with a pong and what the current delay is.")]
        async Task SendPongAsync()
        {
            var msg = await ReplyAsync($"| ~{Context.Client.Latency} ms", ReplyType.Success);
            await msg.ModifySafeAsync(m => m.Content = $"{Res.Str.SuccessText} | {(msg.Timestamp - Context.Message.Timestamp).TotalMilliseconds} ms");
        }
    }
}
