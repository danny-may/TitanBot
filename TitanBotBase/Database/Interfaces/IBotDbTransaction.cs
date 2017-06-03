using System;

namespace TitanBotBase.Database
{
    public interface IBotDbTransaction : IDisposable
    {
        IBotDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IBotDbRecord;
        void Commit();
        void Rollback();
    }
}
