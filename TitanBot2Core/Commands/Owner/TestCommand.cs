using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class TestCommand : Command
    {
        public TestCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(ExecuteAsync);
            RequireOwner = true;
            Description = "Does something, probably";
            Usage.Add("How should I know 0_o");
        }

        private async Task ExecuteAsync(object[] args)
        {
            var data = await Context.WebClient.GetString("http://ipv4.download.thinkbroadband.com:8080/100MB.zip");
            await ReplyAsync(data?.Length.ToString() ?? "null");
        }
    }
}
