using DiscordBot.Logger;
using LiteDB;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Database
{
    public class BotDb : IBotDb
    {
        readonly LiteDatabase _database;
        readonly object _syncLock = new object();
        readonly ILogger _logger;

        public int TotalCalls { get; private set; } = 0;

        public BotDb(string connectionString, ILogger logger)
        {
            _database = new LiteDatabase(connectionString);
            _logger = logger;
        }

        public Task QueryAsync(Action<IBotDbTransaction> query)
            => QueryAsync<object>(conn => { query(conn); return null; });

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
    }
}
