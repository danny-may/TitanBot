using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    [Description("Does something, probably")]
    [Alias("t", "testing")]
    [RequireOwner]
    [Notes("This command changes function very frequently. Dont expect it to be in any way consistent.")]
    public class TestCommand : Command
    {
        [Call]
        [Usage("how should I know 0_o")]
        public async Task ExecuteAsync(double value)
        {
            await ReplyAsync(value.ToString());
        }
    }
}
