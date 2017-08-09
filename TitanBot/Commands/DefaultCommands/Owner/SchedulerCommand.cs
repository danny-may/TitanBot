using System;
using System.Threading.Tasks;
using TitanBot.Replying;
using static TitanBot.TBLocalisation.Commands;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.SCHEDULER)]
    [RequireOwner]
    class SchedulerCommand : Command
    {
        [Call("Prune")]
        [Usage(Usage.SCHEDULER_PRUNE)]
        async Task PruneAsync(DateTime? before = null)
        {
            var beforeDate = before ?? DateTime.Now.AddDays(-7);

            var count = await Scheduler.PruneBefore(beforeDate);

            await ReplyAsync(SchedulerText.PRUNED, ReplyType.Success, count);
        }
    }
}
