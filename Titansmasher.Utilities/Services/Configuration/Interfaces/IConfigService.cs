using System.IO;

namespace Titansmasher.Services.Configuration.Interfaces
{
    public interface IConfigService
    {
        FileInfo ConfigLocation { get; }

        TConfig Request<TConfig>() where TConfig : class, new();

        void Save<TConfig>(TConfig config) where TConfig : class;

        void Refresh();
    }
}