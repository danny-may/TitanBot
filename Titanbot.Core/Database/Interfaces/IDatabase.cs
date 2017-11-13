using System;
using System.Linq.Expressions;

namespace Titanbot.Core.Database.Interfaces
{
    public interface IDatabase
    {
        IDatabaseTable<TRecord> GetTable<TRecord>() where TRecord : IDatabaseRecord;
        void DropTable<TRecord>() where TRecord : IDatabaseRecord;
        void DropTable(string tableName);

        void Backup(DateTime time = default(DateTime));
        void BackupClear(DateTime before = default(DateTime));
        void Shrink();

        IDatabase SetForeignKey<TRecord, TKey>(Expression<Func<TRecord, TKey>> property) where TRecord : IDatabaseRecord where TKey : IDatabaseRecord;
    }
}