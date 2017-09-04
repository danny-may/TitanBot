using LiteDB;
using TitanBot.Core.Services.Database;

namespace TitanBot.Services.Database
{
    internal class DbTransaction : IDbTransaction
    {
        #region Fields

        private readonly LiteDatabase _database;
        private readonly LiteTransaction _transaction;

        #endregion Fields

        #region Constructors

        internal DbTransaction(LiteDatabase database)
        {
            _database = database;
            _transaction = database.BeginTrans();
        }

        #endregion Constructors

        #region Finalisers

        ~DbTransaction()
        {
            Dispose();
        }

        #endregion Finalisers

        #region IDbTransaction

        public IDbTable<TRecord, TId> GetTable<TRecord, TId>() where TRecord : IDbRecord<TId>
            => new DbTable<TRecord, TId>(_database.GetCollection<TRecord>());

        public void Commit()
            => _transaction.Commit();

        public void Dispose()
            => _transaction.Dispose();

        public void Rollback()
            => _transaction.Rollback();

        #endregion IDbTransaction
    }
}