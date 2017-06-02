using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
