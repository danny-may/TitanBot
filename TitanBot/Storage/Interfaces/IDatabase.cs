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
        Task<TReturn> QueryAsync<TReturn>(Func<IDbTransaction, TReturn> query);
        Task QueryTableAsync<TRecord>(Action<IDbTable<TRecord>> query)
            where TRecord : IDbRecord;
        Task<TReturn> QueryTableAsync<TRecord, TReturn>(Func<IDbTable<TRecord>, TReturn> query)
            where TRecord : IDbRecord;

        Task<IEnumerable<TRecord>> Find<TRecord>(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647)
            where TRecord : IDbRecord;
        Task<TRecord> FindOne<TRecord>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord;
        Task<TRecord> FindById<TRecord>(ulong id)
            where TRecord : IDbRecord;
        Task<IEnumerable<TRecord>> FindById<TRecord>(IEnumerable<ulong> ids)
            where TRecord : IDbRecord;
        Task<int> Delete<TRecord>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord;
        Task<bool> Delete<TRecord>(TRecord row)
            where TRecord : IDbRecord;
        Task<bool> Delete<TRecord>(ulong id)
            where TRecord : IDbRecord;
        Task<int> Delete<TRecord>(IEnumerable<TRecord> rows)
            where TRecord : IDbRecord;
        Task<int> Delete<TRecord>(IEnumerable<ulong> ids)
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
        Task<TRecord> GetUpsert<TRecord>(TRecord record)
            where TRecord : IDbRecord;
        Task<IEnumerable<TRecord>> GetUpsert<TRecord>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord;
        Task<TRecord> AddOrGet<TRecord>(ulong Id, TRecord record)
            where TRecord : IDbRecord;
        Task<TRecord> AddOrGet<TRecord>(ulong Id, Func<TRecord> record)
            where TRecord : IDbRecord;
        Task Drop<TRecord>()
            where TRecord : IDbRecord;
        Task Drop(string table);
    }
}
