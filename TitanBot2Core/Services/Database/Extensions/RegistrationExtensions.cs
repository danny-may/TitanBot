using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Tables;
using System.Linq;

namespace TitanBot2.Services.Database.Extensions
{
    public class RegistrationExtensions : DatabaseExtension<Registration>
    {
        public RegistrationExtensions(BotDatabase db) : base(db)
        {
        }

        public async Task<IEnumerable<Registration>> GetForGuild(ulong guildid)
            => await Database.QueryAsync(conn => conn.RegistrationTable.Find(r => r.GuildId == null || r.GuildId == guildid));
        public async Task<IEnumerable<Registration>> GetForUser(ulong userid)
            => await Database.QueryAsync(conn => conn.RegistrationTable.Find(r => r.UserId == userid));
        public async Task<Registration> GetForUserOnGuild(ulong userid, ulong? guildid)
            => await Database.QueryAsync(conn => conn.RegistrationTable.Find(r => r.UserId == userid && r.GuildId == guildid)
                                                                       .OrderBy(r => r.GuildId)
                                                                       .FirstOrDefault());
        public async Task RemoveRegistration(ulong userid, ulong? guildid)
            => await Database.QueryAsync(conn => conn.RegistrationTable.Delete(r => r.UserId == userid && r.GuildId == guildid));
        public async Task RemoveRegistrations(ulong guildid)
            => await Database.QueryAsync(conn => conn.RegistrationTable.Delete(r => r.GuildId == guildid));
    }
}
