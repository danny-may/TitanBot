using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Models;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Services.Database.Extensions
{
    public class TimerExtensions : DatabaseExtension
    {
        public TimerExtensions(TitanbotDatabase db) : base(db) { }

        public async Task Add(Timer timer)
            =>await Database.QueryAsync(conn => conn.TimerTable.Insert(timer));
        public async Task Add(IEnumerable<Timer> timers)
            => await Database.QueryAsync(conn => conn.TimerTable.Insert(timers));

        public Task<IEnumerable<Timer>> Get(ulong guildid)
            => Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && !t.Complete));
        public Task<IEnumerable<Timer>> Get(DateTime activeBefore)
            => Database.QueryAsync(conn => conn.TimerTable.Find(t => t.From < activeBefore && !t.Complete));
        public Task<IEnumerable<Timer>> Get(ulong guildid, EventCallback callback)
            => Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Callback == callback && !t.Complete));

        public async Task Complete(IEnumerable<Timer> timers)
        {
            foreach (var timer in timers)
            {
                timer.Complete = true;
            }

            await Database.QueryAsync(conn => conn.TimerTable.Update(timers));
        }
    }
}
