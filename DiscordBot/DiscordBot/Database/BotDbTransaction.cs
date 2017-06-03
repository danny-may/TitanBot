using LiteDB;

namespace DiscordBot.Database
{
    public class BotDbTransaction : IBotDbTransaction
    {
        private readonly LiteDatabase _database;
        private readonly LiteTransaction _transaction;

        internal BotDbTransaction(LiteDatabase database)
        {
            _database = database;
            _transaction = database.BeginTrans();
        }

        public IBotDbTable<TRecord> GetTable<TRecord>()
            where TRecord : IBotDbRecord
            => new BotDbTable<TRecord>(_database.GetCollection<TRecord>());

        public void Commit()
            => _transaction.Commit();

        public void Dispose()
            => _transaction.Dispose();

        public void Rollback()
            => _transaction.Rollback();
    }
}
