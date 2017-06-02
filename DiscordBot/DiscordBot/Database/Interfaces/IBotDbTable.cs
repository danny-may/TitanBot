using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Database
{
    public interface IBotDbTable<TRecord> 
        where TRecord : IBotDbRecord
    {
        IEnumerable<TRecord> Find(Expression<Func<TRecord, bool>> predicate, int skip = 0, int limit = 2147483647);
        TRecord FindOne(Expression<Func<TRecord, bool>> predicate);
        int Delete(Expression<Func<TRecord, bool>> predicate);
        bool Delete(TRecord row);
        int Delete(IEnumerable<TRecord> rows);
        void Insert(TRecord record);
        void Insert(IEnumerable<TRecord> records);
        void Update(TRecord record);
        void Update(IEnumerable<TRecord> records);
        void Upsert(TRecord record);
        void Upsert(IEnumerable<TRecord> records);
    }
}
