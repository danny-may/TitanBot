using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Extensions
{
    public abstract class DatabaseExtension<T>
    {
        protected TitanbotDatabase Database { get; }
        public DatabaseExtension(TitanbotDatabase db)
        {
            Database = db;
        }

        public async Task Update(IEnumerable<T> objs)
            => await Database.QueryAsync(conn => conn.GetCollection<T>().Update(objs));

        public async Task Update(T obj)
            => await Update(new List<T> { obj });
    }
}
