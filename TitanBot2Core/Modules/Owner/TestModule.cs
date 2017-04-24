using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.Database.Models;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("Test")]
        [Summary("Test command, could be anything, we just dont know :o")]
        public class TestModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Does something probably")]
            public async Task TestAsync()
            {
                var url = "http://www.google.com";
                var req1 = Context.WebService.Get(url);
                var req2 = Context.WebService.Get(url);

                Task.WaitAll(req1, req2);

                var data1 = await req1;
                var data2 = await req1;

                await Context.Channel.SendMessageSafeAsync($"{data1.Length}\n{data2.Length}");
            }
        }
    }
}
