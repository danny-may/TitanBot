using LiteDB;

namespace TitanBotBase.Database
{
    public interface IDbRecord
    {
        [BsonId]
        ulong Id { get; set; }
    }
}
