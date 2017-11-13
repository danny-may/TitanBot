using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Titanbot.Core.Database.Interfaces
{
    public interface IDatabaseTable<TRecord>
        where TRecord : IDatabaseRecord
    {
        List<TRecord> Find(Expression<Func<TRecord, bool>> predicate);
        TRecord Find(decimal id);
        List<TRecord> Find(IEnumerable<decimal> ids);

        void Insert(TRecord record);
        void Insert(IEnumerable<TRecord> records);
        void Insert(params TRecord[] records);

        void Upsert(TRecord record);
        void Upsert(IEnumerable<TRecord> records);
        void Upsert(params TRecord[] records);

        void Update(TRecord record);
        void Update(IEnumerable<TRecord> records);
        void Update(params TRecord[] records);
        void Update(decimal id, Action<TRecord> updater);
        void Update(IEnumerable<decimal> ids, Action<decimal, TRecord> updater);

        void Delete(TRecord record);
        void Delete(IEnumerable<TRecord> records);
        void Delete(params TRecord[] records);
        void Delete(decimal id);
        void Delete(IEnumerable<decimal> ids);
        void Delete(params decimal[] ids);
        void Delete(Expression<Func<TRecord, bool>> predicate);

        List<TRecord> AddOrGet(Expression<Func<TRecord, bool>> predicate, TRecord record);
        List<TRecord> AddOrGet(Expression<Func<TRecord, bool>> predicate, Func<TRecord> record);
        TRecord AddOrGet(decimal id, TRecord record);
        TRecord AddOrGet(decimal id, Func<TRecord> record);
        List<TRecord> AddOrGet(IEnumerable<decimal> ids, Func<decimal, TRecord> records);

        TField Max<TField>(Expression<Func<TRecord, TField>> selector) where TField : IComparable<TField>;
        TField Min<TField>(Expression<Func<TRecord, TField>> selector) where TField : IComparable<TField>;
        decimal Count();
        decimal Count(Expression<Func<TRecord, bool>> selector);
        bool Exists(Expression<Func<TRecord, bool>> selector);
    }
}