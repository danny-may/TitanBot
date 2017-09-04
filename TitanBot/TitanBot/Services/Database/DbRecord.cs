using TitanBot.Core.Services.Database;

namespace TitanBot.Services.Database
{
    public abstract class DbRecord<TId> : IDbRecord<TId>
    {
        public TId Id { get => (TId)_id; set => _id = value; }
        public object _id { get; set; }
    }
}
