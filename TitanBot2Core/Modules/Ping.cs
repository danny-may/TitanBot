using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.Modules
{
    public class Ping : TitanBotModule
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync($"{Resources.Str.SuccessText} | {Context.Client.Latency}ms", ex => Context.TitanBot.Log(ex, "PingCommand"));
        }
    }
}
