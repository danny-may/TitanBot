using LiteDB;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq.Expressions;
using Titanbot.Core.Configuration;
using Titanbot.Core.Database.Interfaces;
using Titanbot.Core.Logging.Interfaces;
using Titansmasher.Extensions;
using static Titanbot.Core.Logging.LogSeverity;

namespace Titanbot.Core.Database
{
    public class Database : IDatabase
    {
        #region Fields

        private LiteDatabase _db;
        private BsonMapper _mapper;
        private Logger _liteLogger;
        private IAreaLogger _logger;
        private FileInfo _location;

        private ConcurrentDictionary<Type, object> _tableCache = new ConcurrentDictionary<Type, object>();

        #endregion Fields

        #region Constructors

        public Database(Config config, ILogger logger)
        {
            _location = new FileInfo(config.Database_Location);
            _logger = logger.CreateAreaLogger<Database>();

            _location.EnsureExists();

            _liteLogger = new Logger(config.Database_LogLevel);
            _mapper = new BsonMapper();
            _db = new LiteDatabase(_location.FullName, _mapper, _liteLogger);

            _mapper.RegisterType(o => (ulong)o, d => (decimal)d);

            _liteLogger.Logging += m => _logger.Log(Verbose, m);

            _logger.Log(Info, "Initialised");
        }

        #endregion Constructors

        #region IDatabase

        public void Backup(DateTime time = default(DateTime))
        {
            Shrink();

            var target = _location.WithTimestamp(time)
                                  .ModifyDirectory(d => Path.Combine(d, "backup"));

            target.EnsureDirectory();

            File.Copy(_location.FullName, target.FullName, true);
        }

        public void BackupClear(DateTime before = default(DateTime))
            => _location.ModifyDirectory(d => Path.Combine(d, "backup"))
                        .Directory
                        .CleanDirectory(before);

        public void DropTable<TRecord>() where TRecord : IDatabaseRecord
            => DropTable(typeof(TRecord).Name);

        public void DropTable(string tableName)
            => _db.DropCollection(tableName);

        public IDatabaseTable<TRecord> GetTable<TRecord>() where TRecord : IDatabaseRecord
            => _tableCache.GetOrAdd(typeof(TRecord), new DatabaseTable<TRecord>(_db.GetCollection<TRecord>())) as DatabaseTable<TRecord>;

        public IDatabase SetForeignKey<TRecord, TKey>(Expression<Func<TRecord, TKey>> property) where TRecord : IDatabaseRecord where TKey : IDatabaseRecord
        {
            _mapper.Entity<TRecord>().DbRef(property);
            (GetTable<TRecord>() as DatabaseTable<TRecord>).Reference(property);
            return this;
        }

        public void Shrink()
            => _db.Shrink();

        #endregion IDatabase
    }
}