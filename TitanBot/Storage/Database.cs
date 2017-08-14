using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TitanBot.Logging;
using TitanBot.Helpers;

namespace TitanBot.Storage
{
    public class Database : IDatabase
    {
        LiteDatabase LiteDatabase { get; }
        ILogger Logger { get; }
        SynchronisedExecutor SyncExec { get; } = new SynchronisedExecutor();

        public int TotalCalls { get; private set; } = 0;

        public Database(ILogger logger) : this(@".\database\bot.db", logger) { }
        public Database(string connectionString, ILogger logger)
        {
            FileUtil.EnsureDirectory(connectionString);

            BsonMapper.Global.RegisterAutoId(u => u == 0, (e, s) =>
            {
                var max = e.Max(s, "_id");
                return max.IsMaxValue ? 1 : (ulong)(max.AsInt64 + 1);
            });

            LiteDatabase = new LiteDatabase(connectionString);
            Logger = logger;
        }

        public Task QueryAsync(Action<IDbTransaction> query)
            => QueryAsync<object>(conn => { query(conn); return null; }).AsTask();

        public async ValueTask<T> QueryAsync<T>(Func<IDbTransaction, T> query)
            => await SyncExec.Run(() =>
            {
                T result = default(T);
                TotalCalls++;
                using (var conn = new DbTransaction(LiteDatabase))
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
                return result;
            });

        public Task QueryTableAsync<T>(Action<IDbTable<T>> query) where T : IDbRecord
            => QueryAsync(conn => query(conn.GetTable<T>()));

        public ValueTask<R> QueryTableAsync<T, R>(Func<IDbTable<T>, R> query) where T : IDbRecord
            => QueryAsync(conn => query(conn.GetTable<T>()));

        public void Dispose()
            => LiteDatabase.Dispose();

        public ValueTask<IEnumerable<T>> All<T>() where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Find(t => true).ToList() as IEnumerable<T>);
        public ValueTask<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Find(predicate, skip, limit).ToList() as IEnumerable<T>);
        public ValueTask<T> FindOne<T>(Expression<Func<T, bool>> predicate) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindOne(predicate));
        public ValueTask<T> FindById<T>(ulong id) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(id));
        public ValueTask<IEnumerable<T>> FindById<T>(IEnumerable<ulong> ids) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().FindById(ids).ToList() as IEnumerable<T>);
        public ValueTask<int> Delete<T>(Expression<Func<T, bool>> predicate) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(predicate));
        public ValueTask<bool> Delete<T>(T row) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(row));
        public ValueTask<bool> Delete<T>(ulong id) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(r => r.Id == id) > 0);
        public ValueTask<int> Delete<T>(IEnumerable<T> rows) where T : IDbRecord
            => QueryAsync(conn => conn.GetTable<T>().Delete(rows));
        public ValueTask<int> Delete<T>(IEnumerable<ulong> ids) where T : IDbRecord
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
        public ValueTask<T> GetUpsert<T>(T record) where T : IDbRecord
            => QueryAsync(conn => { conn.GetTable<T>().Upsert(record); return conn.GetTable<T>().FindById(record.Id); });
        public ValueTask<IEnumerable<T>> GetUpsert<T>(IEnumerable<T> records) where T : IDbRecord
            => QueryAsync(conn => { conn.GetTable<T>().Upsert(records); return conn.GetTable<T>().FindById(records.Select(r => r.Id).ToList()).ToList() as IEnumerable<T>; });

        public ValueTask<T> AddOrGet<T>(ulong id, T record) where T : IDbRecord
            => AddOrGet(id, () => record);

        public ValueTask<T> AddOrGet<T>(ulong id, Func<T> record) where T : IDbRecord
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
            => QueryAsync(conn => LiteDatabase.DropCollection(table)).AsTask();
    }
}
