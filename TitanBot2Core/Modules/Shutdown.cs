using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Modules
{
    public class Shutdown : TitanBotModule
    {
        [Command("shutdown")]
        [RequireOwner]
        [Alias("fuckingdie")]
        public Task ShutdownNowWAsync()
        {
            Task.Run(() => Context.TitanBot.StopAsync());
            return Task.CompletedTask;
        }

        [Command("shutdown")]
        [RequireOwner]
        [Alias("fuckingdie")]
        [Priority(2)]
        public Task ShutdownInWAsync(TimeSpan time)
        {
            Task.Run(() => Context.TitanBot.StopAsync(time));
            return Task.CompletedTask;
        }

        [Command("shutdown")]
        [RequireOwner]
        [Alias("fuckingdie")]
        [Priority(1)]
        public Task ShutdownNowWAsync([Remainder]string reason)
        {
            Task.Run(() => Context.TitanBot.StopAsync(reason: reason));
            return Task.CompletedTask;
        }

        [Command("shutdown")]
        [RequireOwner]
        [Alias("fuckingdie")]
        [Priority(3)]
        public Task ShutdownInWAsync(TimeSpan time, [Remainder]string reason)
        {
            Task.Run(() => Context.TitanBot.StopAsync(time, reason));
            return Task.CompletedTask;
        }
    }
}
