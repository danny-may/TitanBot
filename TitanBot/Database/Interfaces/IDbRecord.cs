using LiteDB;

namespace TitanBot.Database
{
    public interface IDbRecord
    {
        [BsonId]
        ulong Id { get; set; }
    }
}
