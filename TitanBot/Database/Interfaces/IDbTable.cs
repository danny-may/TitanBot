using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TitanBot.Database
{
    public interface IDbTable<TRecord>
        where TRecord : IDbRecord
    {
        IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647);
        TRecord FindOne(Expression<Func<TRecord, bool>> predicate);
        TRecord FindById(ulong id);
        IEnumerable<TRecord> FindById(IEnumerable<ulong> ids);
        int Delete(Expression<Func<TRecord, bool>> predicate);
        bool Delete(TRecord row);
        bool Delete(ulong id);
        int Delete(IEnumerable<TRecord> rows);
        int Delete(IEnumerable<ulong> ids);
        void Insert(TRecord record);
        void Insert(IEnumerable<TRecord> records);
        void Update(TRecord record);
        void Update(IEnumerable<TRecord> records);
        void Upsert(TRecord record);
        void Upsert(IEnumerable<TRecord> records);
        void Ensure(TRecord record);
        void Ensure(IEnumerable<TRecord> records);
    }
}
