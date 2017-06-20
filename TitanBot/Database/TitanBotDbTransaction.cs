using LiteDB;

namespace TitanBot.Database
{
    class TitanBotDbTransaction : IDbTransaction
    {
        private LiteDatabase Database { get; }
        private LiteTransaction Transaction { get; }

        internal TitanBotDbTransaction(LiteDatabase database)
        {
            Database = database;
            Transaction = database.BeginTrans();
        }

        public IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord
            => new TitanBotDbTable<TRecord>(Database.GetCollection<TRecord>());

        public void Commit()
            => Transaction.Commit();

        public void Dispose()
            => Transaction.Dispose();

        public void Rollback()
            => Transaction.Rollback();
    }
}
