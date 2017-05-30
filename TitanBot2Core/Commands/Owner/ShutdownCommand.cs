using System;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    [Description("Allows my owner to shut me down")]
    [RequireOwner]
    class ShutdownCommand : Command
    {
        [Call]
        [Usage("Sets a timer going for the bot to shut down, along with a reason")]
        async Task ShutDown(TimeSpan? time = null, string reason = null)
        {
            await ReplyAsync("Starting shut down sequence!");
            Context.BotClient.StopAsync(time, reason);
        }
    }
}
