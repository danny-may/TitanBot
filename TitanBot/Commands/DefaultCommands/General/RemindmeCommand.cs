using System;
using System.Threading.Tasks;
using TitanBot.Callbacks;
using TitanBot.Formatting;
using TitanBot.Replying;

namespace TitanBot.Commands.DefaultCommands.General
{
    //class RemindMeCommand : Command
    //{
    //    [Call]
    //    [Alias("in")]
    //    async Task RemindMeInAsync(TimeSpan duration, [Dense]string message)
    //    {
    //        Scheduler.Queue<RemindMeHandler>(Author.Id, Guild?.Id, DateTime.Now, null, DateTime.Now.Add(duration), Message.Id, Channel.Id, message);
    //        await ReplyAsync(new RawString("Ill remind you in {0}", ReplyType.Success, duration));
    //    }
    //}
}
