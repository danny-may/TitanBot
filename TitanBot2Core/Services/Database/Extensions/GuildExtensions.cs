using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database.Extensions
{

    public class GuildExtensions : DatabaseExtension
    {
        public GuildExtensions(TitanbotDatabase db) : base(db) { }

        public async Task<Guild> GetGuild(ulong guildId)
            => await GetGuild(guildId, null);
        public async Task<Guild> GetGuild(ulong guildId, Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.GuildTable.FindOne(g => g.GuildId == guildId), handler);

        public async Task<string> GetPrefix(ulong guildId)
            => await GetPrefix(guildId, null);
        public async Task<string> GetPrefix(ulong guildId, Func<Exception, Task> handler)
            =>(await GetGuild(guildId, handler))?.Prefix ?? Configuration.Instance.Prefix;
           

        public async Task EnsureExists(ulong guildId)
            => await EnsureExists(guildId, null);
        public async Task EnsureExists(ulong guildId, Func<Exception, Task> handler)
        {
            if (await GetGuild(guildId) == null)
                await Database.QueryAsync(conn => conn.GuildTable.Insert(new Guild { GuildId = guildId }));
        }

        public async Task<IEnumerable<ulong>> GetAliveChannels()
            => await GetAliveChannels(null);
        public async Task<IEnumerable<ulong>> GetAliveChannels(Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.GuildTable
                                               .Find(g => g.NotifyAlive != null)
                                               .Select(g => g.NotifyAlive.Value)
                                               .ToList());

        public async Task<IEnumerable<ulong>> GetDeadChannels()
            => await GetDeadChannels(null);
        public async Task<IEnumerable<ulong>> GetDeadChannels(Func<Exception, Task> handler)
            => await Database.QueryAsync(conn => conn.GuildTable
                                               .Find(g => g.NotifyDead != null)
                                               .Select(g => g.NotifyDead.Value)
                                               .ToList());
    }
}
