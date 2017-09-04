using System;

namespace TitanBot.Core.Services.Database
{
    public interface IDbTransaction : IDisposable
    {
        IDbTable<TRecord, TId> GetTable<TRecord, TId>()
            where TRecord : IDbRecord<TId>;

        void Commit();

        void Rollback();
    }
}