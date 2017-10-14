using LiteDB;
using System.Reflection;
using TitanBot.Core.Services.Database;

namespace TitanBot.Services.Database
{
    public abstract class DbRecord : IDbRecord
    {
        [BsonId(true)]
        public ulong Id { get; set; }

        public static string IdName = ReflectionExtensions.GetMemberName<IDbRecord, ulong>(r => r.Id);
    }
}