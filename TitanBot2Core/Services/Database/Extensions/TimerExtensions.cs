using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Tables;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Services.Database.Extensions
{
    public class TimerExtensions : DatabaseExtension<Timer>
    {
        public TimerExtensions(BotDatabase db) : base(db) { }

        public async Task Add(Timer timer)
            =>await Database.QueryAsync(conn => conn.TimerTable.Insert(timer));
        public async Task Add(IEnumerable<Timer> timers)
            => await Database.QueryAsync(conn => conn.TimerTable.Insert(timers));

        public Task<IEnumerable<Timer>> GetActive()
            => Database.QueryAsync(conn => conn.TimerTable.Find(t => t.Active && t.From < DateTime.Now));

        public Task<IEnumerable<Timer>> Get(ulong guildid, bool includeInactive = false)
        {
            if (includeInactive)
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid));
            else
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Active));
        }

        public Task<IEnumerable<Timer>> Get(DateTime activeBefore, bool includeInactive = false)
        {
            if (includeInactive)
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.From < activeBefore));
            else
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.From < activeBefore && t.Active));
        }

        public Task<IEnumerable<Timer>> Get(ulong guildid, EventCallback callback, bool includeInactive = false)
        {
            if (includeInactive)
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Callback == callback));
            else
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Callback == callback && t.Active));
        }

        public Task<Timer> GetLatest(ulong guildid, EventCallback callback, bool includeInactive = false)
        {
            if (includeInactive)
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Callback == callback).OrderBy(t => t.To ?? DateTime.Now).LastOrDefault());
            else
                return Database.QueryAsync(conn => conn.TimerTable.Find(t => t.GuildId == guildid && t.Callback == callback && t.Active).OrderBy(t => t.To ?? DateTime.Now).LastOrDefault());
        }

        public async Task Complete(IEnumerable<Timer> timers)
        {
            foreach (var timer in timers)
            {
                timer.Complete = true;
                timer.To = DateTime.Now;
            }

            await Database.Timers.Update(timers);
        }

        public async Task Cancel (IEnumerable<Timer> timers)
        {
            foreach (var timer in timers)
            {
                timer.Cancelled = true;
            }

            await Database.Timers.Update(timers);
        }
    }
}
