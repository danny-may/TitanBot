using System;
using System.Threading.Tasks;

namespace TitanBot.Core.Services.Database
{
    public interface IDatabaseService : IDisposable
    {
        int TotalCalls { get; }

        Task QueryAsync(Action<IDbTransaction> query);

        ValueTask<TReturn> QueryAsync<TReturn>(Func<IDbTransaction, TReturn> query);

        void Query(Action<IDbTransaction> query);

        TReturn Query<TReturn>(Func<IDbTransaction, TReturn> query);

        Task QueryTableAsync<TRecord, TId>(Action<IDbTable<TRecord, TId>> query)
            where TRecord : IDbRecord<TId>;

        ValueTask<TReturn> QueryTableAsync<TRecord, TId, TReturn>(Func<IDbTable<TRecord, TId>, TReturn> query)
            where TRecord : IDbRecord<TId>;

        void QueryTable<TRecord, TId>(Action<IDbTable<TRecord, TId>> query)
            where TRecord : IDbRecord<TId>;

        TReturn QueryTable<TRecord, TId, TReturn>(Func<IDbTable<TRecord, TId>, TReturn> query)
            where TRecord : IDbRecord<TId>;

        Task Drop<TRecord, TId>()
            where TRecord : IDbRecord<TId>;

        Task Drop(string table);
    }
}