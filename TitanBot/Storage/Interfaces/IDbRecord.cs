using LiteDB;

namespace TitanBot.Storage
{
    public interface IDbRecord
    {
        [BsonId]
        ulong Id { get; set; }
    }
}
