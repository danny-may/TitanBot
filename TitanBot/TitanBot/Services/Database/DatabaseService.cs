using LiteDB;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Logging;
using TitanBot.Utility;

namespace TitanBot.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        #region Statics

        public static readonly string IdName = ReflectionExtensions.GetMemberName<IDbRecord<object>, object>(r => r.Id);

        #endregion Statics

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

            BsonMapper.Global.RegisterAutoId(u => u == 0, (e, s) =>
            {
                var max = e.Max(s, "_id");
                return max.IsMaxValue ? 1 : (ulong)(max.AsInt64 + 1);
            });

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

        #region IDatabaseService

        public int TotalCalls { get; private set; }

        public void Dispose()
            => _database.Dispose();

        public Task Drop<TRecord, TId>() where TRecord : IDbRecord<TId>
            => Drop(typeof(TRecord).Name);

        public Task Drop(string table)
            => _processor.Run(() => _database.DropCollection(table));

        public void Query(Action<IDbTransaction> query)
            => QueryAsync(query).Wait();

        public TReturn Query<TReturn>(Func<IDbTransaction, TReturn> query)
            => QueryAsync(query).Result;

        public async Task QueryAsync(Action<IDbTransaction> query)
            => await QueryAsync(conn => { query(conn); return 0; });

        public async ValueTask<TReturn> QueryAsync<TReturn>(Func<IDbTransaction, TReturn> query)
            => await _processor.Run(() =>
            {
                var result = default(TReturn);
                TotalCalls++;
                using (var conn = new DbTransaction(_database))
                {
                    try
                    {
                        result = query(conn);
                        conn.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(ex, "DatabaseService");
                        conn.Rollback();
                    }
                }
                return result;
            });

        public void QueryTable<TRecord, TId>(Action<IDbTable<TRecord, TId>> query) where TRecord : IDbRecord<TId>
            => QueryTableAsync(query).Wait();

        public TReturn QueryTable<TRecord, TId, TReturn>(Func<IDbTable<TRecord, TId>, TReturn> query) where TRecord : IDbRecord<TId>
            => QueryTableAsync(query).Result;

        public async Task QueryTableAsync<TRecord, TId>(Action<IDbTable<TRecord, TId>> query) where TRecord : IDbRecord<TId>
            => await QueryTableAsync<TRecord, TId, int>(conn => { query(conn); return 0; });

        public async ValueTask<TReturn> QueryTableAsync<TRecord, TId, TReturn>(Func<IDbTable<TRecord, TId>, TReturn> query) where TRecord : IDbRecord<TId>
            => await QueryAsync(conn => query(conn.GetTable<TRecord, TId>()));

        #endregion IDatabaseService
    }
}