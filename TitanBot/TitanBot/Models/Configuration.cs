using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TitanBot.Core.Models;

namespace TitanBot.Models
{
    public sealed class TBConfig : Configuration<TBConfig>
    {
    }

    public abstract class Configuration<T> : IConfiguration where T : Configuration<T>, new()
    {
        [JsonIgnore]
        public static string FileName = "./config.json";

        [JsonIgnore]
        private static FileInfo _file => new FileInfo(Path.Combine(AppContext.BaseDirectory, FileName));

        public string Token { get; set; } = "";
        public string Prefix { get; set; } = "t$";
        public ulong[] Owners { get; set; } = new ulong[0];

        public void Save()
            => Save(this);

        public void Refresh()
        {
            var updated = Load();
            foreach (var property in GetType().GetProperties())
                if (property.GetValue(this) != property.GetValue(updated))
                    property.SetValue(this, property.GetValue(updated));
        }

        private static void EnsureDirectory()
        {
            if (!_file.Directory.Exists)
                _file.Directory.Create();
        }

        public static void Save(Configuration<T> config)
        {
            EnsureDirectory();
            File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public static Configuration<T> Load()
        {
            EnsureDirectory();
            if (!_file.Exists)
                Save(new T());
            var config = JsonConvert.DeserializeObject<T>(_file.OpenText().ReadToEnd());
            if (config == null)
            {
                config = new T();
                Save(config);
            }
            return config;
        }
    }
}