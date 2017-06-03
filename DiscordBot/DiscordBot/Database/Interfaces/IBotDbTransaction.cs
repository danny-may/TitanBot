using System;

namespace DiscordBot.Database
{
    public interface IBotDbTransaction : IDisposable
    {
        IBotDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IBotDbRecord;
        void Commit();
        void Rollback();
    }
}
