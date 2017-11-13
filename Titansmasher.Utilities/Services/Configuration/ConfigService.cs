using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Titansmasher.Extensions;
using Titansmasher.Services.Configuration.Interfaces;

namespace Titansmasher.Services.Configuration
{
    public class ConfigService : IConfigService
    {
        #region Statics

        public static FileInfo Location = new FileInfo("./config.json");

        #endregion Statics

        #region Fields

        public FileInfo ConfigLocation => Location;
        private JObject _data;
        private JsonSerializer _serialiser;

        #endregion Fields

        public ConfigService()
        {
            Refresh();
            _serialiser = JsonSerializer.CreateDefault();
            _serialiser.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            _serialiser.Error += _serialiser_Error;
        }

        private void _serialiser_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            if (e.ErrorContext.Error.Message == "Serializing delegates is not supported on this platform.")
                e.ErrorContext.Handled = true;
        }

        #region IConfigService

        public void Refresh()
        {
            Location.EnsureExists("{}");
            _data = JObject.Parse(Location.ReadAllText());
        }

        public void Save<TConfig>(TConfig config) where TConfig : class
        {
            _data[typeof(TConfig).Name] = JToken.FromObject(config, _serialiser);

            Location.EnsureDirectory();
            Location.WriteAllText(_data.ToString(Formatting.Indented));
        }

        public TConfig Request<TConfig>() where TConfig : class, new()
        {
            if (_data.TryGetValue(typeof(TConfig).Name, out var value))
                return value.ToObject<TConfig>(_serialiser);

            var config = new TConfig();
            Save(config);

            return config;
        }

        #endregion IConfigService
    }
}