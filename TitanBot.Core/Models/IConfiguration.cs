namespace TitanBot.Core.Models
{
    public interface IConfiguration
    {
        ulong[] Owners { get; set; }
        string Prefix { get; set; }
        string Token { get; set; }

        void Save();

        void Refresh();
    }
}