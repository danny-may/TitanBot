using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Core.Services.Database
{
    public interface IDatabaseService
    {
        int TotalCalls { get; }

        Task QueryAsync(Action<IDbTransaction> query);

        ValueTask<TReturn> QueryAsync<TReturn>(Func<IDbTransaction, TReturn> query);

        Task QueryTableAsync<TRecord, TId>(Action<IDbTable<TRecord, TId>> query)
            where TRecord : IDbRecord<TId>;

        ValueTask<TReturn> QueryTableAsync<TRecord, TId, TReturn>(Func<IDbTable<TRecord, TId>, TReturn> query)
            where TRecord : IDbRecord<TId>;

        ValueTask<IEnumerable<TRecord>> All<TRecord, TId>()
            where TRecord : IDbRecord<TId>;

        ValueTask<IEnumerable<TRecord>> Find<TRecord, TId>(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647)
            where TRecord : IDbRecord<TId>;

        ValueTask<TRecord> FindOne<TRecord, TId>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord<TId>;

        ValueTask<TRecord> FindById<TRecord, TId>(ulong id)
            where TRecord : IDbRecord<TId>;

        ValueTask<IEnumerable<TRecord>> FindById<TRecord, TId>(IEnumerable<ulong> ids)
            where TRecord : IDbRecord<TId>;

        ValueTask<int> Delete<TRecord, TId>(Expression<Func<TRecord, bool>> predicate)
            where TRecord : IDbRecord<TId>;

        ValueTask<bool> Delete<TRecord, TId>(TRecord row)
            where TRecord : IDbRecord<TId>;

        ValueTask<bool> Delete<TRecord, TId>(ulong id)
            where TRecord : IDbRecord<TId>;

        ValueTask<int> Delete<TRecord, TId>(IEnumerable<TRecord> rows)
            where TRecord : IDbRecord<TId>;

        ValueTask<int> Delete<TRecord, TId>(IEnumerable<ulong> ids)
            where TRecord : IDbRecord<TId>;

        Task Insert<TRecord, TId>(TRecord record)
            where TRecord : IDbRecord<TId>;

        Task Insert<TRecord, TId>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord<TId>;

        Task Update<TRecord, TId>(TRecord record)
            where TRecord : IDbRecord<TId>;

        Task Update<TRecord, TId>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord<TId>;

        Task Upsert<TRecord, TId>(TRecord record)
            where TRecord : IDbRecord<TId>;

        Task Upsert<TRecord, TId>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord<TId>;

        ValueTask<TRecord> GetUpsert<TRecord, TId>(TRecord record)
            where TRecord : IDbRecord<TId>;

        ValueTask<IEnumerable<TRecord>> GetUpsert<TRecord, TId>(IEnumerable<TRecord> records)
            where TRecord : IDbRecord<TId>;

        ValueTask<TRecord> AddOrGet<TRecord, TId>(ulong Id, TRecord record)
            where TRecord : IDbRecord<TId>;

        ValueTask<TRecord> AddOrGet<TRecord, TId>(ulong Id, Func<TRecord, TId> record)
            where TRecord : IDbRecord<TId>;

        Task Drop<TRecord, TId>()
            where TRecord : IDbRecord<TId>;

        Task QueryTableAsync<TRecord>(Action<IDbTable<TRecord>> query)
            where TRecord : IDbRecord;

        ValueTask<TReturn> QueryTableAsync<TRecord, TReturn>(Func<IDbTable<TRecord>, TReturn> query)
            where TRecord : IDbRecord;

        ValueTask<IEnumerable<TRecord>> All<TRecord>()
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