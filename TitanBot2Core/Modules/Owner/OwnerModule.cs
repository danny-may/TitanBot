using Discord.Commands;
using System;
using System.Threading.Tasks;
using TitanBot2.TypeReaders;

namespace TitanBot2.Modules.Owner
{
    [Name("Owner")]
    [RequireOwner]
    public partial class OwnerModule : TitanBotModule
    {
        [Command("Test", RunMode = RunMode.Async)]
        public async Task TestAsync([OverrideTypeReader(typeof(BetterTimespanTypeReader))]TimeSpan time)
        {
            await ReplyAsync(time.ToString());
        }
    }
}
