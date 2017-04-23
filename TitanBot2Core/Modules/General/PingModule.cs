using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Preconditions;

namespace TitanBot2.Modules.General
{
    public partial class GeneralModule
    {
        [Group("Ping")]
        [Summary("Gets a ping from the bot to check the current delay")]
        public class PingModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Gets the current delay")]
            [RequireCustomPermission(0)]
            public async Task PingAsync()
            {
                var msg = await ReplyAsync($"{Res.Str.SuccessText} | ~{Context.Client.Latency} ms", ex => Context.Logger.Log(ex, "PingCmd"));
                await msg.ModifySafeAsync(m => m.Content = $"{Res.Str.SuccessText} | {(msg.Timestamp - Context.Message.Timestamp).TotalMilliseconds} ms", ex => Context.Logger.Log(ex, "PingCmd"));
            }
        }
    }
}
