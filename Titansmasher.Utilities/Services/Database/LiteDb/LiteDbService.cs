using LiteDB;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq.Expressions;
using Titansmasher.Extensions;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;

namespace Titansmasher.Services.Database.LiteDb
{
    public class LiteDbService : IDatabaseService
    {
        #region Fields

        private LiteDatabase _db;
        private BsonMapper _mapper;
        private Logger _liteLogger;
        private ILoggerService _logger;
        private FileInfo _location;
        private LiteDbConfig _config;

        private ConcurrentDictionary<Type, object> _tableCache = new ConcurrentDictionary<Type, object>();

        #endregion Fields

        #region Constructors

        public LiteDbService(LiteDbConfig config, ILoggerService logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _location = new FileInfo(_config.Location);
            _location.EnsureExists();

            _liteLogger = new Logger(_config.LogLevel);
            _mapper = new BsonMapper();
            _db = new LiteDatabase(_location.FullName, _mapper, _liteLogger);

            _mapper.RegisterType(o => (ulong)o, d => (decimal)d);

            _liteLogger.Logging += m => _logger.Log(LogLevel.Verbose, m);

            _logger.Log(LogLevel.Info, "Initialised");
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
            => _tableCache.GetOrAdd(typeof(TRecord), new LiteDbTable<TRecord>(_db.GetCollection<TRecord>())) as LiteDbTable<TRecord>;

        public IDatabaseService SetForeignKey<TRecord, TKey>(Expression<Func<TRecord, TKey>> property) where TRecord : IDatabaseRecord where TKey : IDatabaseRecord
        {
            _mapper.Entity<TRecord>().DbRef(property);
            (GetTable<TRecord>() as LiteDbTable<TRecord>).Reference(property);
            return this;
        }

        public void Shrink()
            => _db.Shrink();

        #endregion IDatabase
    }
}