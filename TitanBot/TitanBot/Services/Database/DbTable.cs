using LiteDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Core.Services.Database;

namespace TitanBot.Services.Database
{
    internal class DbTable
    {
        #region Statics

        internal readonly static ConcurrentDictionary<Type, bool> _mapped = new ConcurrentDictionary<Type, bool>();

        #endregion Statics
    }

    internal class DbTable<TRecord, TId> : DbTable, IDbTable<TRecord, TId> where TRecord : IDbRecord<TId>
    {
        #region Fields

        private readonly LiteCollection<TRecord> _collection;

        #endregion Fields

        #region Constructors

        internal DbTable(LiteCollection<TRecord> collection)
        {
            _mapped.GetOrAdd(typeof(TRecord), t =>
            {
                BsonMapper.Global.Entity<TRecord>()
                                 .Ignore(r => r.Id)
                                 .Id(r => r._id, true);
                return true;
            });
            _collection = collection;
        }

        #endregion Constructors

        #region IDbTable

        public IEnumerable<TRecord> All()
            => _collection.FindAll();

        public int Delete(Expression<Func<TRecord, bool>> predicate)
            => _collection.Delete(predicate);

        public bool Delete(TRecord row)
            => Delete(row.Id);

        public bool Delete(TId id)
            => _collection.Delete(new BsonValue(id));

        public int Delete(IEnumerable<TRecord> rows)
            => Delete(rows.Select(r => r.Id));

        public int Delete(IEnumerable<TId> ids)
            => _collection.Delete(Query.In(DatabaseService.IdName, ids.ToBson()));

        public IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = int.MaxValue)
            => _collection.Find(predicate, skip, limit);

        public TRecord FindById(TId id)
            => _collection.FindById(new BsonValue(id));

        public IEnumerable<TRecord> FindById(IEnumerable<TId> ids)
            => _collection.Find(Query.In(DatabaseService.IdName, ids.ToBson()));

        public TRecord FindOne(Expression<Func<TRecord, bool>> predicate)
            => FindOne(predicate);

        public TRecord GetOrAdd(TId id, Func<TId, TRecord> func)
            => GetOrAdd(new[] { id }, func).First();

        public IEnumerable<TRecord> GetOrAdd(IEnumerable<TId> ids, Func<TId, TRecord> func)
        {
            var existing = FindById(ids).ToList();
            var created = ids.Where(i => !existing.Exists(r => Equals(i, r.Id)))
                             .Select(r => func(r));
            Insert(created);
            return existing.Concat(created);
        }

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

        #endregion IDbTable
    }
}