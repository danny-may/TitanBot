using System;

namespace TitanBotBase.Database
{
    public interface IDbTransaction : IDisposable
    {
        IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord;
        void Commit();
        void Rollback();
    }
}
