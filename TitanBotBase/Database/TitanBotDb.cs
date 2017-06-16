using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TitanBotBase.Logger;
using TitanBotBase.Util;

namespace TitanBotBase.Database
{
    public class TitanBotDb : IDatabase
    {
        LiteDatabase Database { get; }
        object SyncLock { get; } = new object();
        ILogger Logger { get; }

        public int TotalCalls { get; private set; } = 0;

        public TitanBotDb(ILogger logger) : this(@".\database\bot.db", logger) { }
        public TitanBotDb(string connectionString, ILogger logger)
        {
            FileUtil.EnsureDirectory(connectionString);

            BsonMapper.Global.RegisterAutoId(u => u == 0, (e, s) => (ulong)DateTime.UtcNow.Ticks);

            Database = new LiteDatabase(connectionString);
            Logger = logger;
        }

        public Task QueryAsync(Action<IDbTransaction> query)
            => QueryAsync<object>(conn => { query(conn); return null; });

        public Task<T> QueryAsync<T>(Func<IDbTransaction, T> query)
        {
            return Task.Run(() =>
            {
                T result = default(T);
                lock (SyncLock)
                {
                    TotalCalls++;
                    using (var conn = new TitanBotDbTransaction(Database))
                    {
                        try
                        {
                            result = query(conn);
                            conn.Commit();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(ex, "DatabaseQuery");
                            conn.Rollback();
                        }
                    }
                }
                return result;
            });
        }

        public Task QueryTableAsync<T>(Action<IDbTable<T>> query) where T : IDbRecord
            => QueryAsync(conn => query(conn.GetTable<T>()));

        public Task<R> QueryTableAsync<T, R>(Func<IDbTable<T>, R> query) where T : IDbRecord
            => QueryAsync(conn => query(conn.GetTable<T>()));

        public void Dispose()
            => Database.Dispose();

        public Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue)where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Find(predicate, skip, limit).ToList() as IEnumerable<T>);
        public Task<T> FindOne<T>(Expression<Func<T, bool>> predicate) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindOne(predicate));
        public Task<T> FindById<T>(ulong id) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(id));
        public Task<IEnumerable<T>> FindById<T>(IEnumerable<ulong> ids) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(ids).ToList() as IEnumerable<T>);
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
            => QueryAsync(conn => conn.GetTable<T>().Upsert(records));
        public Task<T> GetUpsert<T>(T record) where T : IDbRecord
            => QueryAsync(conn => { conn.GetTable<T>().Upsert(record); return conn.GetTable<T>().FindById(record.Id); });
        public Task<IEnumerable<T>> GetUpsert<T>(IEnumerable<T> records) where T : IDbRecord
            => QueryAsync(conn => { conn.GetTable<T>().Upsert(records); return conn.GetTable<T>().FindById(records.Select(r => r.Id).ToList()).ToList() as IEnumerable<T>; });

        public Task<T> AddOrGet<T>(ulong id, T record) where T : IDbRecord
            => AddOrGet(id, () => record);

        public Task<T> AddOrGet<T>(ulong id, Func<T> record) where T : IDbRecord
        {
            return QueryAsync(conn =>
            {
                var current = conn.GetTable<T>().FindOne(r => r.Id == id);
                if (!current?.Equals(default(T)) ?? false)
                    return current;
                current = record();
                current.Id = id;
                conn.GetTable<T>().Insert(current);
                return current;
            });
        }

        public Task Drop<T>() where T : IDbRecord
            => Drop(typeof(T).Name);

        public Task Drop(string table)
            => QueryAsync(conn => Database.DropCollection(table));
    }
}
