using System;
using System.Linq.Expressions;

namespace Titansmasher.Services.Database.Interfaces
{
    public interface IDatabaseService
    {
        IDatabaseTable<TRecord> GetTable<TRecord>() where TRecord : IDatabaseRecord;
        void DropTable<TRecord>() where TRecord : IDatabaseRecord;
        void DropTable(string tableName);

        void Backup(DateTime time = default);
        void BackupClear(DateTime before = default);
        void Shrink();

        IDatabaseService SetForeignKey<TRecord, TKey>(Expression<Func<TRecord, TKey>> property) where TRecord : IDatabaseRecord where TKey : IDatabaseRecord;
    }
}