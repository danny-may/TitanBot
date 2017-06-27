using System;

namespace TitanBot.Storage
{
    public interface IDbTransaction : IDisposable
    {
        IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord;
        void Commit();
        void Rollback();
    }
}
