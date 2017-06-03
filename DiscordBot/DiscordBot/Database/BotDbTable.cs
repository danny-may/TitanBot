using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DiscordBot.Database
{
    public class BotDbTable<TRecord> : IBotDbTable<TRecord>
        where TRecord : IBotDbRecord
    {
        private readonly LiteCollection<TRecord> _collection;

        internal BotDbTable(LiteCollection<TRecord> collection)
        {
            _collection = collection;
        }

        public int Delete(Expression<Func<TRecord, bool>> predicate)
            => _collection.Delete(predicate);
        public bool Delete(TRecord row)
            => _collection.Delete(r => r.GetHashCode() == row.GetHashCode()) > 0;
        public int Delete(IEnumerable<TRecord> rows)
            => rows.Select(r => Delete(r)).Count(r => r);
        public IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647)
            => _collection.Find(predicate, skip, limit);
        public TRecord FindOne(Expression<Func<TRecord, bool>> predicate)
            => _collection.FindOne(predicate);
        public void Insert(TRecord record)
            => _collection.Insert(record);
        public void Insert(IEnumerable<TRecord> records)
            => _collection.Insert(records);
        public void Update(TRecord record)
            => _collection.Update(record);
        public void Update(IEnumerable<TRecord> records)
            => _collection.Update(records);
        public void Upsert(TRecord record)
            => _collection.Upsert(record);
        public void Upsert(IEnumerable<TRecord> records)
            => _collection.Upsert(records);
    }
}
