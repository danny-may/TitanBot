using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TitanBot.Storage
{
    public interface IDatabase : IDisposable
    {
        int TotalCalls { get; }

        Task QueryAsync(Action<IDbTransaction> query);
        ValueTask<TReturn> QueryAsync<TReturn>(Func<IDbTransaction, TReturn> query);
        Task QueryTableAsync<TRecord>(Action<IDbTable<TRecord>> query)
            where TRecord : IDbRecord;
        ValueTask<TReturn> QueryTableAsync<TRecord, TReturn>(Func<IDbTable<TRecord>, TReturn> query)
            where TRecord : IDbRecord;

        ValueTask<IEnumerable<TRecord>> Find<TRecord>(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647)
            where TRecord : IDbRecord;
        ValueTask<TRecord> FindOne<TRecord>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord;
        ValueTask<TRecord> FindById<TRecord>(ulong id)
            where TRecord : IDbRecord;
        ValueTask<IEnumerable<TRecord>> FindById<TRecord>(IEnumerable<ulong> ids)
            where TRecord : IDbRecord;
        ValueTask<int> Delete<TRecord>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord;
        ValueTask<bool> Delete<TRecord>(TRecord row)
            where TRecord : IDbRecord;
        ValueTask<bool> Delete<TRecord>(ulong id)
            where TRecord : IDbRecord;
        ValueTask<int> Delete<TRecord>(IEnumerable<TRecord> rows)
            where TRecord : IDbRecord;
        ValueTask<int> Delete<TRecord>(IEnumerable<ulong> ids)
            where TRecord : IDbRecord;
        Task Insert<TRecord>(TRecord record)
            where TRecord : IDbRecord;
        Task Insert<TRecord>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord;
        Task Update<TRecord>(TRecord record)
            where TRecord : IDbRecord;
        Task Update<TRecord>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord;
        Task Upsert<TRecord>(TRecord record)
            where TRecord : IDbRecord;
        Task Upsert<TRecord>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord;
        ValueTask<TRecord> GetUpsert<TRecord>(TRecord record)
            where TRecord : IDbRecord;
        ValueTask<IEnumerable<TRecord>> GetUpsert<TRecord>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord;
        ValueTask<TRecord> AddOrGet<TRecord>(ulong Id, TRecord record)
            where TRecord : IDbRecord;
        ValueTask<TRecord> AddOrGet<TRecord>(ulong Id, Func<TRecord> record)
            where TRecord : IDbRecord;
        Task Drop<TRecord>()
            where TRecord : IDbRecord;
        Task Drop(string table);
    }
}
