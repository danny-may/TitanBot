using TitanBotBase.Logger;
using LiteDB;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TitanBotBase.Database
{
    public class TitanBotDb : IDatabase
    {
        readonly LiteDatabase _database;
        readonly object _syncLock = new object();
        readonly ILogger _logger;

        public int TotalCalls { get; private set; } = 0;

        public TitanBotDb(string connectionString, ILogger logger)
        {
            _database = new LiteDatabase(connectionString);
            _logger = logger;
        }

        public Task QueryAsync(Action<IDbTransaction> query)
            => QueryAsync<object>(conn => { query(conn); return null; });

        public Task<T> QueryAsync<T>(Func<IDbTransaction, T> query)
        {
            return Task.Run(() =>
            {
                T result = default(T);
                lock (_syncLock)
                {
                    TotalCalls++;
                    using (var conn = new TitanBotDbTransaction(_database))
                    {
                        try
                        {
                            result = query(conn);
                            conn.Commit();
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(ex, "DatabaseQuery");
                            conn.Rollback();
                        }
                    }
                }
                return result;
            });
        }

        public void Dispose()
            => _database.Dispose();

        public Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue)where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Find(predicate, skip, limit));
        public Task<T> FindOne<T>(Expression<Func<T, bool>> predicate) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindOne(predicate));
        public Task<T> FindById<T>(ulong id) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(id));
        public Task<IEnumerable<T>> FindById<T>(IEnumerable<ulong> ids) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(ids));
        public Task<int> Delete<T>(Expression<Func<T, bool>> predicate) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(predicate));
        public Task<bool> Delete<T>(T row) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(row));
        public Task<bool> Delete<T>(ulong id) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(id));
        public Task<int> Delete<T>(IEnumerable<T> rows) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(rows));
        public Task<int> Delete<T>(IEnumerable<ulong> ids) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(ids));
        public Task Insert<T>(T record) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Insert(record));
        public Task Insert<T>(IEnumerable<T> records) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Insert(records));
        public Task Update<T>(T record) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Update(record));
        public Task Update<T>(IEnumerable<T> records) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Update(records));
        public Task Upsert<T>(T record) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Upsert(record));
        public Task Upsert<T>(IEnumerable<T> records) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(records));
    }
}
