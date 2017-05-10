using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using TitanBot2.Services.Database.Extensions;

namespace TitanBot2.Services.Database
{
    public partial class TitanbotDatabase : ThreadedService
    {
        private string FileName { get; set; }

        private DbConnection _db;
        private ConcurrentQueue<Task> _queryQueue;
        public event Func<Exception, Task> DefaultExceptionHandler;

        public GuildExtensions Guilds { get; }
        public CmdPermExtensions CmdPerms { get; }
        public TimerExtensions Timers { get; }
        public UserExtensions Users { get; }
        public ExcuseExtension Excuses { get; }
        public RegistrationExtensions Registrations { get; }

        public TitanbotDatabase(string target)
        {
            FileName = target;
            Guilds = new GuildExtensions(this);
            CmdPerms = new CmdPermExtensions(this);
            Timers = new TimerExtensions(this);
            Users = new UserExtensions(this);
            Excuses = new ExcuseExtension(this);
            Registrations = new RegistrationExtensions(this);
        }

        public override void Initialise()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            string path = Path.GetDirectoryName(file);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DefaultExceptionHandler += ex => Task.CompletedTask;
            _queryQueue = new ConcurrentQueue<Task>();
            _db = new DbConnection(file);

            base.Initialise();
        }

        protected override async Task Main(DateTime loopTime)
        {
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

        private async Task<T> QueueQuery<T>(Func<DbConnection, T> query, Func<Exception, Task> handler)
        {
            T result = default(T);

            if (query == null)
                return result;

            var task = new Task(async () =>
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
                        await (handler?.Invoke(ex) ?? Task.CompletedTask);
                    }
                }
            });

            _queryQueue.Enqueue(task);

            await task;

            return result;
        }

        public async Task QueryAsync(Action<DbConnection> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public async Task<T> QueryAsync<T>(Func<DbConnection, T> query)
            => await QueryAsync(query, DefaultExceptionHandler);

        public async Task QueryAsync(Action<DbConnection> query, Func<Exception, Task> handler)
        {
            await QueueQuery<object>(db => { query(db); return null; }, handler ?? DefaultExceptionHandler);
        }

        public async Task<T> QueryAsync<T>(Func<DbConnection, T> query, Func<Exception, Task> handler)
        {
            return await QueueQuery(query, handler ?? DefaultExceptionHandler);
        }
    }
}
