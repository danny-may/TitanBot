using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Database
{
    class BotDb : IBotDb
    {
        readonly LiteDatabase _database;
        readonly object _syncLock = new object();

        public int TotalCalls { get; private set; } = 0;

        public BotDb(string connectionString)
        {
            _database = new LiteDatabase(connectionString);
        }

        public Task QueryAsync(Action<IBotDbTransaction> query)
        {
            return Task.Run(() =>
            {
                lock (_syncLock)
                {
                    TotalCalls++;
                    using (var conn = new BotDbTransaction(_database))
                    {
                        try
                        {
                            query(conn);
                            conn.Commit();
                        }
                        catch (Exception)
                        {
                            //TODO: Add logger here
                            conn.Rollback();
                        }
                    }
                }
            });
        }

        public Task<T> QueryAsync<T>(Func<IBotDbTransaction, T> query)
        {
            return Task.Run(() =>
            {
                T result = default(T);
                lock (_syncLock)
                {
                    TotalCalls++;
                    using (var conn = new BotDbTransaction(_database))
                    {
                        try
                        {
                            result = query(conn);
                            conn.Commit();
                        }
                        catch (Exception)
                        {
                            //TODO: Add logger here
                            conn.Rollback();
                        }
                    }
                }
                return result;
            });
        }

        public void Dispose()
            => _database.Dispose();
    }
}
