using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Database
{
    public interface IDbTransaction : IDisposable
    {
        IDbTable<TRecord, TId> GetTable<TRecord, TId>()
            where TRecord : IDbRecord<TId>;

        IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord;

        void Commit();

        void Rollback();
    }
}