using System;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Owner
{
    public class ShutdownCommand : Command
    {
        public ShutdownCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Calls.AddNew(a => ShutdownAsync());
            Calls.AddNew(a => ShutdownAsync((TimeSpan)a[0]))
                 .WithArgTypes(typeof(TimeSpan));
            Calls.AddNew(a => ShutdownAsync((TimeSpan)a[0], (string)a[1]))
                 .WithArgTypes(typeof(TimeSpan), typeof(string))
                 .WithItemAsParams(1);
            Calls.AddNew(a => ShutdownAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0);

            RequireOwner = true;
            Usage.Add("`{0} [time] [reason]` Sets a timer going for the bot to shut down, along with a reason");
            Description = "Allows my owner to shut me down";
        }

        public Task ShutdownAsync()
                => ShutDown();

        public Task ShutdownAsync(TimeSpan time)
                => ShutDown(time);

        public Task ShutdownAsync(string reason)
                => ShutDown(reason: reason);

        public Task ShutdownAsync(TimeSpan time, string reason)
                => ShutDown(time, reason);

        private async Task ShutDown(TimeSpan? time = null, string reason = null)
        {
            await ReplyAsync("Starting shut down sequence!");
            Context.TitanBot.StopAsync(time, reason);
        }
    }
}
