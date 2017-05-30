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
        [RequireContext(ContextType.Guild)]
        [CallFlag("f", "flag", @"¯\_(ツ)_/¯")]
        public async Task ExecuteAsync(TimeSpan? time = null, [Dense]string text = "default text")
        {
            var message = "";
            message += $"Prefix: {Context.Prefix}\n";
            message += $"Command: {Context.Command}\n";
            message += $"Arg1: {time} ({typeof(TimeSpan?)})\n";
            message += $"Arg2: {text} ({typeof(string)})\n";
            message += $"Flag(F): {Flags.Has("f")}\n";

            await Task.Delay(30000);

            await ReplyAsync(message);
        }
    }
}
