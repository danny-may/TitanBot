using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TitanBotBase.Database
{
    public class TitanBotDbTable<TRecord> : IDbTable<TRecord>
        where TRecord : IDbRecord
    {
        private readonly LiteCollection<TRecord> _collection;

        internal TitanBotDbTable(LiteCollection<TRecord> collection)
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
        public TRecord FindById(ulong id)
            => _collection.FindById(id);
        public IEnumerable<TRecord> FindById(IEnumerable<ulong> ids)
            => ids.Select(i => FindById(i)).ToList();
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
        public bool Delete(ulong id)
            => _collection.Delete(id);
        public int Delete(IEnumerable<ulong> ids)
            => ids.Select(i => Delete(i)).Count(r => r);
    }
}
