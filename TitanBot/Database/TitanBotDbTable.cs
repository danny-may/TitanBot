using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Util;

namespace TitanBot.Database
{
    class TitanBotDbTable<TRecord> : IDbTable<TRecord>
        where TRecord : IDbRecord
    {
        private LiteCollection<TRecord> Collection { get; }

        internal TitanBotDbTable(LiteCollection<TRecord> collection)
        {
            Collection = collection;
        }

        public int Delete(Expression<Func<TRecord, bool>> predicate)
            => Collection.Delete(predicate);
        public bool Delete(TRecord row)
            => Collection.Delete(row.Id);
        public int Delete(IEnumerable<TRecord> rows)
            => rows.Select(r => Delete(r)).Count(r => r);
        public IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647)
            => Collection.Find(predicate, skip, limit);
        public TRecord FindOne(Expression<Func<TRecord, bool>> predicate)
            => Collection.FindOne(predicate);
        public TRecord FindById(ulong id)
            => Collection.FindOne(r => r.Id == id);
        public IEnumerable<TRecord> FindById(IEnumerable<ulong> ids)
            => ids.Select(i => FindById(i)).ToList();
        public void Insert(TRecord record)
            => Collection.Insert(record);
        public void Insert(IEnumerable<TRecord> records)
            => Collection.Insert(records);
        public void Update(TRecord record)
            => Collection.Update(record);
        public void Update(IEnumerable<TRecord> records)
            => Collection.Update(records);
        public void Upsert(TRecord record)
            => Collection.Upsert(record);
        public void Upsert(IEnumerable<TRecord> records)
            => Collection.Upsert(records);
        public bool Delete(ulong id)
            => Collection.Delete(id);
        public int Delete(IEnumerable<ulong> ids)
            => ids.Select(i => Delete(i)).Count(r => r);
        public void Ensure(TRecord record)
            => Collection.Upsert(Collection.FindOne(r => r.Id == record.Id).IfDefault(record));
        public void Ensure(IEnumerable<TRecord> records)
            => records.ToList().ForEach(r => Ensure(r));
    }
}
