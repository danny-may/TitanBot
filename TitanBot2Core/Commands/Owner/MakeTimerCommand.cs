using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.Database.Tables;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Commands.Owner
{
    [Description("Forces a timer object into the database")]
    [RequireOwner]
    class MakeTimerCommand : Command
    {
        [Call]
        [Usage("Makes and inserts a timer into the database")]
        async Task AddTimerAsync(EventCallback callback, 
                                 IMessageChannel channel,
                                 IUser user,
                                 int pollingPeriod)
        {
            if (!Flags.TryGet("d", out TimeSpan? duration))
                duration = null;

            var timer = new Timer
            {
                Callback = callback,
                ChannelId = channel.Id,
                UserId = user.Id,
                From = DateTime.Now,
                To = duration == null ? (DateTime?)null : DateTime.Now.Add(duration.Value),
                GuildId = Context.Guild.Id,
                MessageId = Context.Message.Id,
                SecondInterval = pollingPeriod
            };

            await Context.Database.Timers.Add(timer);

            await ReplyAsync("Successfully inserted timer", ReplyType.Success);
        }
    }
}
