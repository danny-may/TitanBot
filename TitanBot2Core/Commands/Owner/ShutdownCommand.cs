using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                => Context.TitanBot.StopAsync();

        public Task ShutdownAsync(TimeSpan time)
                => Context.TitanBot.StopAsync(time);

        public Task ShutdownAsync(string reason)
                => Context.TitanBot.StopAsync(reason: reason);

        public Task ShutdownAsync(TimeSpan time, string reason)
                => Context.TitanBot.StopAsync(time, reason);
    }
}
