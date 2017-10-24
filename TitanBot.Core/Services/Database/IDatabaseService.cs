using System;

namespace TitanBot.Core.Services.Database
{
    public interface IDatabaseService : IDisposable
    {
        int TotalCalls { get; }

        void Query<TRecord>(Action<IDbTable<TRecord>> action) where TRecord : IDbRecord;

        TResult Query<TRecord, TResult>(Func<IDbTable<TRecord>, TResult> action) where TRecord : IDbRecord;

        TRecord Query<TRecord>(Func<IDbTable<TRecord>, TRecord> action) where TRecord : IDbRecord;

        void Drop<TRecord>() where TRecord : IDbRecord;
        void Drop(string tableName);
    }
}