using LiteDB;

namespace TitanBot.Storage
{
    class DbTransaction : IDbTransaction
    {
        private LiteDatabase Database { get; }
        private LiteTransaction Transaction { get; }

        internal DbTransaction(LiteDatabase database)
        {
            Database = database;
            Transaction = database.BeginTrans();
        }

        public IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord
            => new DbTable<TRecord>(Database.GetCollection<TRecord>());

        public void Commit()
            => Transaction.Commit();

        public void Dispose()
            => Transaction.Dispose();

        public void Rollback()
            => Transaction.Rollback();
    }
}
