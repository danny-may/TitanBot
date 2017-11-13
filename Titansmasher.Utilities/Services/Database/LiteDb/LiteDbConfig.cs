using LiteDB;
using Titansmasher.Services.Database.Interfaces;

namespace Titansmasher.Services.Database.LiteDb
{
    public class LiteDbConfig : DatabaseConfig
    {
        public byte LogLevel { get; set; } = Logger.FULL;
    }
}