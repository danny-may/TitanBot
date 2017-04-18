using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database.Models;

namespace TitanBot2.Database
{

    public class GuildExtensions
    {
        internal GuildExtensions() { }

        public async Task<Guild> GetGuild(ulong guildId)
            => await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.FindOne(g => g.GuildId == guildId));
        public async Task<Guild> GetGuild(ulong guildId, Func<Exception, Task> handler)
            => await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.FindOne(g => g.GuildId == guildId), handler);

        public async Task<string> GetPrefix(ulong guildId)
            => (await GetGuild(guildId))?.Prefix ?? Configuration.Instance.Prefix;
        public async Task<string> GetPrefix(ulong guildId, Func<Exception, Task> handler)
            => (await GetGuild(guildId, handler))?.Prefix ?? Configuration.Instance.Prefix;

        public async Task EnsureExists(ulong guildId)
        {
            if (await GetGuild(guildId) == null)
                await TitanbotDatabase.QueryAsync(conn => conn.GuildTable.Insert(new Guild { GuildId = guildId }));
        }
    }
}
