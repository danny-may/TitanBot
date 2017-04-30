using LiteDB;
using System;
using System.IO;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database
{
    public class DbConnection : LiteDatabase
    {
        internal DbConnection(ConnectionString connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
        {
        }

        internal DbConnection(string connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
        {
        }

        internal DbConnection(Stream stream, BsonMapper mapper = null, string password = null) : base(stream, mapper, password)
        {
        }

        internal DbConnection(IDiskService diskService, BsonMapper mapper = null, string password = null, TimeSpan? timeout = default(TimeSpan?), int cacheSize = 5000, Logger log = null) : base(diskService, mapper, password, timeout, cacheSize, log)
        {
        }

        public LiteCollection<Guild> GuildTable { get { return GetCollection<Guild>(); } }
        public LiteCollection<CmdPerm> CmdPermTable { get { return GetCollection<CmdPerm>(); } }
        public LiteCollection<Timer> TimerTable { get { return GetCollection<Timer>(); } }
        public LiteCollection<User> UserTable { get { return GetCollection<User>(); } }
        public LiteCollection<Excuse> ExcuseTable { get { return GetCollection<Excuse>(); } }
    }
}
