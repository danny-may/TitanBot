using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace TitanBot2.Modules
{
    public class Test : TitanBotModule
    {
        [Command("test")]
        public async Task TestAsync(TimeSpan ts)
        {
            await ReplyAsync(ts.ToString(), ex => Context.TitanBot.Log(ex, "TestCommand"));
        }
    }
}
