using Discord.Commands;
using System;
using System.Threading.Tasks;
using TitanBot2.TypeReaders;

namespace TitanBot2.Modules.Owner
{
    public partial class OwnerModule
    {
        [Group("Shutdown"), Alias("Fuckingdie")]
        [Summary("Tells the bot to shutdown.")]
        public class ShutdownModule : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            [Remarks("Shuts the bot down after the given time and with the given reason")]
            public Task ShutdownAsync(TimeSpan time = new TimeSpan(), [Remainder]string reason = null)
                => Context.TitanBot.StopAsync(time, reason);
        }
    }
}
