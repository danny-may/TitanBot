using System;
using TitanBot.Contexts;
using TitanBot.Formatting;
using TitanBot.Scheduling;

namespace TitanBot.Callbacks
{
    class RemindMeHandler : ISchedulerCallback
    {
        public async void Complete(ISchedulerContext context, bool wasCancelled)
        {
            var message = context.Record.Data;
            if (string.IsNullOrWhiteSpace(message))
                return;
            var channel = await context.Author?.GetOrCreateDMChannelAsync();
            if (channel == null)
                return;

            var timespan = DateTime.Now - context.Record.StartTime;
            await context.Replier.Reply(channel, context.Author).WithMessage(new RawString("{0} ago you asked me to remind you this:\n{1}", timespan, message)).SendAsync();
        }

        public void Handle(ISchedulerContext context, DateTime eventTime)
        {
            throw new NotImplementedException();
        }
    }
}
