namespace TitanBot.Core.Services.Database
{
    public interface IDbRecord<TId> : IDbRecord
    {
        TId Id { get; set; }
    }

    public interface IDbRecord
    {
        object _id { get; set; }
    }
}