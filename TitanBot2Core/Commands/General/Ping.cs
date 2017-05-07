using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class Ping : Command
    {
        public Ping(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Usage = new string[]
            {
                "`{0}` - Replies with a pong and what the current delay is."
            };

            Description = "Basic command for calculating the delay of the bot.";
        }

        protected override async Task RunAsync()
        {
            var msg = await ReplyAsync($"{Res.Str.SuccessText} | ~{Context.Client.Latency} ms", ex => Context.Logger.Log(ex, "PingCmd"));
            await msg.ModifySafeAsync(m => m.Content = $"{Res.Str.SuccessText} | {(msg.Timestamp - Context.Message.Timestamp).TotalMilliseconds} ms", ex => Context.Logger.Log(ex, "PingCmd"));
        }
    }
}
