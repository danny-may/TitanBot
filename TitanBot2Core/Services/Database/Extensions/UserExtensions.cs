using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database.Extensions
{
    public class UserExtensions : DatabaseExtension<User>
    {
        public UserExtensions(BotDatabase db) : base(db)
        {
        }

        public async Task<User> Find(ulong userid)
            => await Database.QueryAsync(conn => conn.UserTable.Find(u => u.DiscordId == userid).SingleOrDefault());

        public async Task<User> Find(string supportCode)
            => await Database.QueryAsync(conn => conn.UserTable.Find(u => u.SupportCode == supportCode).SingleOrDefault());
    }
}
