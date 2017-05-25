using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TitanBot2.Services.Database.Extensions
{
    public abstract class DatabaseExtension<T>
    {
        protected BotDatabase Database { get; }
        public DatabaseExtension(BotDatabase db)
        {
            Database = db;
        }

        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate)
            => await Database.QueryAsync(conn => conn.GetCollection<T>().Find(predicate));

        public async Task Update(IEnumerable<T> objs)
            => await Database.QueryAsync(conn => conn.GetCollection<T>().Update(objs));

        public async Task Update(T obj)
            => await Update(new List<T> { obj });

        public async Task Insert(IEnumerable<T> objs)
            => await Database.QueryAsync(conn => conn.GetCollection<T>().Insert(objs));

        public async Task Insert(T obj)
            => await Insert(new List<T> { obj });

        public async Task Upsert(IEnumerable<T> objs)
            => await Database.QueryAsync(conn => conn.GetCollection<T>().Upsert(objs));

        public async Task Upsert(T obj)
            => await Upsert(new List<T> { obj });

        public async Task Drop()
            => await Database.QueryAsync(conn => conn.DropCollection(typeof(T).Name));
    }
}
