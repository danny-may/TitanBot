using LiteDB;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Database.Models;
using System.Collections.Concurrent;

namespace TitanBot2.Database
{
    public partial class TitanbotDatabase
    {
        public static string FileName { get; } = "database/TitanBot2.db";

        private DbConnection _db;
        private Task _worker;
        private ConcurrentQueue<Task> _queryQueue;
        public static event Func<Exception, Task> DefaultExceptionHandler;

        private static TitanbotDatabase _instance;
        private static TitanbotDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TitanbotDatabase();
                return _instance;
            }
        }

        private TitanbotDatabase()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            string path = Path.GetDirectoryName(file);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DefaultExceptionHandler += ex => Task.CompletedTask;
            _queryQueue = new ConcurrentQueue<Task>();
            _db = new DbConnection(file);
            _worker = new Task(async () =>
            {
                while (true)
                {
                    await Task.Delay(1);
                    while (_queryQueue.Count > 0)
                    {
                        Task item;
                        if (_queryQueue.TryDequeue(out item))
                        {
                            item.Start();
                            await item;
                        }
                    }
                }
            });

            _worker.Start();
        }

        private async Task<T> QueueQuery<T>(Func<DbConnection, T> query, Func<Exception, Task> handler)
        {
            T result = default(T);

            if (query == null)
                return result;

            var task = new Task(async db =>
            {
                using (var t = (db as DbConnection).BeginTrans())
                {
                    try
                    {
                        result = query(db as DbConnection);
                        t.Commit();
                    }
                    catch (Exception ex)
                    {
                        t.Rollback();
                        await (handler?.Invoke(ex) ?? Task.CompletedTask);
                    }
                }
            }, _db);

            _queryQueue.Enqueue(task);

            await task;

            return result;
        }

        public static async Task QueryAsync(Action<DbConnection> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public static async Task<T> QueryAsync<T>(Func<DbConnection, T> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public static async Task QueryAsync(Action<DbConnection> query, Func<Exception, Task> handler)
        {
            await Instance.QueueQuery<object>(db => { query(db); return null; }, handler);
        }

        public static async Task<T> QueryAsync<T>(Func<DbConnection, T> query, Func<Exception, Task> handler)
        {
            return await Instance.QueueQuery(query, handler);
        }

        public static GuildExtensions Guilds { get; } = new GuildExtensions();
    }
}
