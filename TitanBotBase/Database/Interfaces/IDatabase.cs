using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TitanBotBase.Database
{
    public interface IDatabase : IDisposable
    {
        int TotalCalls { get; }

        Task QueryAsync(Action<IDbTransaction> query);
        Task<T> QueryAsync<T>(Func<IDbTransaction, T> query);
        Task QueryTableAsync<T>(Action<IDbTable<T>> query)
            where T : IDbRecord;
        Task<R> QueryTableAsync<T,R>(Func<IDbTable<T>, R> query)
            where T : IDbRecord;

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647)
            where T : IDbRecord;
        Task<T> FindOne<T>(Expression<Func<T, bool>> predicate)
            where T : IDbRecord;
        Task<T> FindById<T>(ulong id)
            where T : IDbRecord;
        Task<IEnumerable<T>> FindById<T>(IEnumerable<ulong> ids)
            where T : IDbRecord;
        Task<int> Delete<T>(Expression<Func<T, bool>> predicate)
            where T : IDbRecord;
        Task<bool> Delete<T>(T row)
            where T : IDbRecord;
        Task<bool> Delete<T>(ulong id)
            where T : IDbRecord;
        Task<int> Delete<T>(IEnumerable<T> rows)
            where T : IDbRecord;
        Task<int> Delete<T>(IEnumerable<ulong> ids)
            where T : IDbRecord;
        Task Insert<T>(T record)
            where T : IDbRecord;
        Task Insert<T>(IEnumerable<T> records)
            where T : IDbRecord;
        Task Update<T>(T record)
            where T : IDbRecord;
        Task Update<T>(IEnumerable<T> records)
            where T : IDbRecord;
        Task Upsert<T>(T record)
            where T : IDbRecord;
        Task Upsert<T>(IEnumerable<T> records)
            where T : IDbRecord;
        Task<T> GetUpsert<T>(T record)
            where T : IDbRecord;
        Task<IEnumerable<T>> GetUpsert<T>(IEnumerable<T> records)
            where T : IDbRecord;
        Task<T> AddOrGet<T>(ulong Id, T record)
            where T : IDbRecord;
        Task<T> AddOrGet<T>(ulong Id, Func<T> record)
            where T : IDbRecord;
        Task Drop<T>()
            where T : IDbRecord;
        Task Drop(string table);
    }
}
