using System;

namespace TitanBot.Database
{
    public interface IDbTransaction : IDisposable
    {
        IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord;
        void Commit();
        void Rollback();
    }
}
