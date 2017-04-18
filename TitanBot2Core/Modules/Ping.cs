using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;

namespace TitanBot2.Modules
{
    public class Ping : TitanBotModule
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            var msg = await ReplyAsync($"{Res.Str.SuccessText} | ~{Context.Client.Latency} ms", ex => Context.Logger.Log(ex, "PingCmd"));
            await msg.ModifyAsync(m => m.Content = $"{Res.Str.SuccessText} | {(msg.Timestamp - Context.Message.Timestamp).TotalMilliseconds} ms", ex => Context.Logger.Log(ex, "PingCmd"));
        }
    }
}
