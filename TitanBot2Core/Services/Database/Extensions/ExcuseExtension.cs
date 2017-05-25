using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database.Extensions
{
    public class ExcuseExtension : DatabaseExtension<Excuse>
    {
        public ExcuseExtension(BotDatabase db) : base(db)
        {
        }

        public async Task<Excuse> GetRandom()
        {
            var excuses = await Database.QueryAsync(conn => conn.ExcuseTable.FindAll());

            var rand = new Random();
            return excuses.ToList()[rand.Next(excuses.Count())];
        }

        public async Task<Excuse> Get(int id)
            => await Database.QueryAsync(conn => conn.ExcuseTable.FindOne(ex => ex.ExcuseNo == id));

        public async Task Delete(Excuse obj)
            => await Delete(new List<Excuse> { obj });
        public async Task Delete(IEnumerable<Excuse> objs)
        {
            await Database.QueryAsync(conn =>
            {
                foreach (var item in objs)
                {
                    conn.ExcuseTable.Delete(item.ExcuseNo);
                }
            });
        }
    }
}
