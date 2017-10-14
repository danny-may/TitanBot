using LiteDB;
using System;
using System.IO;
using System.Reflection;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Logging;
using TitanBot.Utility;

namespace TitanBot.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        #region Fields

        private readonly LiteDatabase _database;
        private readonly ILoggerService _logger;
        private readonly FileInfo _file;
        private readonly ProcessingQueue _processor = new ProcessingQueue { ProcessOnThread = true };

        #endregion Fields

        #region Constructors

        public DatabaseService(ILoggerService logger) : this(logger, "./database/bot.db")
        {
        }

        public DatabaseService(ILoggerService logger, string dbLocation)
        {
            _file = new FileInfo(Path.Combine(AppContext.BaseDirectory, dbLocation));
            FileExtensions.EnsureDirectory(_file.Directory);

            _database = new LiteDatabase(_file.FullName);
            _logger = logger;
        }

        #endregion Constructors

        #region Finalisers

        ~DatabaseService()
        {
            Dispose();
        }

        #endregion Finalisers

        #region Methods

        private TResult RunQuery<TRecord, TResult>(Func<IDbTable<TRecord>, TResult> action) where TRecord : IDbRecord
        {
            var table = _database.GetCollection<TRecord>();
            return action(new DbTable<TRecord>(table));
        }

        private void RunQuery<TRecord>(Action<IDbTable<TRecord>> action) where TRecord : IDbRecord
            => RunQuery<TRecord, bool>(t => { action(t); return true; });

        #endregion Methods

        #region IDatabaseService

        public int TotalCalls { get; private set; }

        public void Dispose()
            => _database.Dispose();

        public void Drop<TRecord>() where TRecord : IDbRecord
            => Drop(typeof(TRecord).Name);

        public void Drop(string tableName)
            => _database.DropCollection(tableName);

        public void Query<TRecord>(Action<IDbTable<TRecord>> action) where TRecord : IDbRecord
            => RunQuery(action);

        public TResult Query<TRecord, TResult>(Func<IDbTable<TRecord>, TResult> action) where TRecord : IDbRecord
            => RunQuery(action);

        public TRecord Query<TRecord>(Func<IDbTable<TRecord>, TRecord> action) where TRecord : IDbRecord
            => RunQuery(action);

        #endregion IDatabaseService
    }
}