using LiteDB;

namespace TitanBotBase.Database
{
    public class TitanBotDbTransaction : IDbTransaction
    {
        private readonly LiteDatabase _database;
        private readonly LiteTransaction _transaction;

        internal TitanBotDbTransaction(LiteDatabase database)
        {
            _database = database;
            _transaction = database.BeginTrans();
        }

        public IDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IDbRecord
            => new TitanBotDbTable<TRecord>(_database.GetCollection<TRecord>());

        public void Commit()
            => _transaction.Commit();

        public void Dispose()
            => _transaction.Dispose();

        public void Rollback()
            => _transaction.Rollback();
    }
}
