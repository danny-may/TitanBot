using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Titansmasher.Services.Database.Interfaces;

namespace Titansmasher.Services.Database.LiteDb
{
    internal class LiteDbTable<TRecord> : IDatabaseTable<TRecord> where TRecord : IDatabaseRecord
    {
        #region Fields

        private LiteCollection<TRecord> _collection;

        #endregion Fields

        #region Constructors

        internal LiteDbTable(LiteCollection<TRecord> collection)
        {
            _collection = collection;
        }

        #endregion Constructors

        #region Methods

        private Query IdsIn(IEnumerable<decimal> ids)
            => Query.In(nameof(IDatabaseRecord.Id), ids.Select(i => (BsonValue)i));

        internal void Reference<TId>(Expression<Func<TRecord, TId>> path)
            => _collection = _collection.Include(path);

        #endregion Methods

        #region IDatabaseTable

        #region AddOrGet

        public List<TRecord> AddOrGet(Expression<Func<TRecord, bool>> predicate, TRecord record)
            => AddOrGet(predicate, () => record);

        public List<TRecord> AddOrGet(Expression<Func<TRecord, bool>> predicate, Func<TRecord> record)
        {
            var existing = Find(predicate);

            if (existing.Count == 0)
            {
                existing.Add(record());
                Upsert(existing[0]);
            }

            return existing;
        }

        public TRecord AddOrGet(decimal id, TRecord record)
            => AddOrGet(id, () => record);

        public TRecord AddOrGet(decimal id, Func<TRecord> record)
            => AddOrGet(new[] { id }, i => record())[0];

        public List<TRecord> AddOrGet(IEnumerable<decimal> ids, Func<decimal, TRecord> records)
        {
            var existing = Find(ids);
            var added = ids.Where(i => !existing.Exists(r => r.Id == i))
                           .Select(i => { var r = records(i); r.Id = i; return r; });

            Upsert(added);

            return ids.Join(existing.Concat(added), i => i, r => r.Id, (i, r) => (i: i, r: r))
                      .Select(p => p.r)
                      .ToList();
        }

        #endregion AddOrGet

        #region Delete

        public void Delete(TRecord record)
            => Delete(record.Id);

        public void Delete(IEnumerable<TRecord> records)
            => Delete(records.ToArray());

        public void Delete(params TRecord[] records)
            => Delete(records.Select(r => r.Id));

        public void Delete(decimal id)
            => _collection.Delete(id);

        public void Delete(IEnumerable<decimal> ids)
            => Delete(ids.ToArray());

        public void Delete(params decimal[] ids)
            => _collection.Delete(IdsIn(ids));

        public void Delete(Expression<Func<TRecord, bool>> predicate)
            => _collection.Delete(predicate);

        #endregion Delete

        #region Find

        public List<TRecord> Find(Expression<Func<TRecord, bool>> predicate)
            => _collection.Find(predicate).ToList();

        public TRecord Find(decimal id)
            => _collection.FindById(id);

        public List<TRecord> Find(IEnumerable<decimal> ids)
            => _collection.Find(IdsIn(ids)).ToList();

        #endregion Find

        #region Insert

        public void Insert(TRecord record)
            => _collection.Insert(record);

        public void Insert(IEnumerable<TRecord> records)
            => _collection.Insert(records);

        public void Insert(params TRecord[] records)
            => _collection.Insert(records);

        #endregion Insert

        #region Update

        public void Update(TRecord record)
            => _collection.Update(record);

        public void Update(IEnumerable<TRecord> records)
            => _collection.Update(records);

        public void Update(params TRecord[] records)
            => _collection.Update(records);

        public void Update(decimal id, Action<TRecord> updater)
            => Update(new[] { id }, (i, r) => updater(r));

        public void Update(IEnumerable<decimal> ids, Action<decimal, TRecord> updater)
        {
            var records = Find(ids);

            foreach (var record in records)
                updater(record.Id, record);

            Update(records);
        }

        #endregion Update

        #region Upsert

        public void Upsert(TRecord record)
            => _collection.Upsert(record);

        public void Upsert(IEnumerable<TRecord> records)
            => _collection.Upsert(records);

        public void Upsert(params TRecord[] records)
            => _collection.Upsert(records);

        #endregion Upsert

        #region Counts

        public TField Max<TField>(Expression<Func<TRecord, TField>> selector) where TField : IComparable<TField>
            => throw new NotImplementedException();

        public TField Min<TField>(Expression<Func<TRecord, TField>> selector) where TField : IComparable<TField>
            => throw new NotImplementedException();

        public decimal Count()
            => (decimal)_collection.LongCount();

        public decimal Count(Expression<Func<TRecord, bool>> selector)
            => (decimal)_collection.LongCount(selector);

        #endregion Counts

        public bool Exists(Expression<Func<TRecord, bool>> selector)
            => _collection.Exists(selector);

        #endregion IDatabaseTable
    }
}