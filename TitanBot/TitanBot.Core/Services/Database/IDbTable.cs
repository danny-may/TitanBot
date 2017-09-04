using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TitanBot.Core.Services.Database
{
    public interface IDbTable<TRecord, TId>
           where TRecord : IDbRecord<TId>
    {
        IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647);

        IEnumerable<TRecord> All();

        TRecord FindOne(Expression<Func<TRecord, bool>> predicate);

        TRecord FindById(TId id);

        IEnumerable<TRecord> FindById(IEnumerable<TId> ids);

        int Delete(Expression<Func<TRecord, bool>> predicate);

        bool Delete(TRecord row);

        bool Delete(TId id);

        int Delete(IEnumerable<TRecord> rows);

        int Delete(IEnumerable<TId> ids);

        void Insert(TRecord record);

        void Insert(IEnumerable<TRecord> records);

        void Update(TRecord record);

        void Update(IEnumerable<TRecord> records);

        void Upsert(TRecord record);

        void Upsert(IEnumerable<TRecord> records);

        TRecord GetOrAdd(TId id, Func<TId, TRecord> func);

        IEnumerable<TRecord> GetOrAdd(IEnumerable<TId> ids, Func<TId, TRecord> func);
    }
}