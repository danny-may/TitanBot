using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.Database.Extensions;

namespace TitanBot2.Services.Database
{
    public partial class BotDatabase
    {
        private string FileName { get; set; }

        private DbConnection _db;
        private object _lock = new object();
        public event Func<Exception, Task> DefaultExceptionHandler;

        public GuildExtensions Guilds { get; }
        public CmdPermExtensions CmdPerms { get; }
        public TimerExtensions Timers { get; }
        public UserExtensions Users { get; }
        public ExcuseExtension Excuses { get; }
        public RegistrationExtensions Registrations { get; }

        public int TotalCalls { get; private set; } = 0;

        public BotDatabase(string target)
        {
            FileName = target;
            Guilds = new GuildExtensions(this);
            CmdPerms = new CmdPermExtensions(this);
            Timers = new TimerExtensions(this);
            Users = new UserExtensions(this);
            Excuses = new ExcuseExtension(this);
            Registrations = new RegistrationExtensions(this);

            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            string path = Path.GetDirectoryName(file);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DefaultExceptionHandler += ex => Task.CompletedTask;
            _db = new DbConnection(file);
        }

        public async Task QueryAsync(Action<DbConnection> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public async Task<T> QueryAsync<T>(Func<DbConnection, T> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public Task QueryAsync(Action<DbConnection> query, Func<Exception, Task> handler)
        {
            TotalCalls++;
            return Task.Run(() =>
            {
                lock (_db)
                {
                    using (var t = _db.BeginTrans())
                    {
                        try
                        {
                            query(_db);
                            t.Commit();
                        }
                        catch (Exception ex)
                        {
                            t.Rollback();
                            (handler?.Invoke(ex) ?? Task.CompletedTask).Wait();
                        }
                    }
                }
            });
        }

        public Task<T> QueryAsync<T>(Func<DbConnection, T> query, Func<Exception, Task> handler)
        {
            TotalCalls++;
            return Task.Run(() =>
            {
                T result = default(T);
                lock (_db)
                {
                    using (var t = _db.BeginTrans())
                    {
                        try
                        {
                            result = query(_db);
                            t.Commit();
                        }
                        catch (Exception ex)
                        {
                            t.Rollback();
                            (handler?.Invoke(ex) ?? Task.CompletedTask).Wait();
                        }
                    }
                }
                return result;
            });
        }
    }
}
