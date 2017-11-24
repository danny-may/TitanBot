using System.Collections.Generic;
using Titansmasher.Services.Database.Interfaces;

namespace Titanbot.Settings.Models
{
    public class SettingCollectionRecord : IDatabaseRecord
    {
        public decimal Id { get; set; }
        public Dictionary<string, object> Settings
        {
            get => _settings;
            set => _settings = value ?? new Dictionary<string, object>();
        }

        private Dictionary<string, object> _settings = new Dictionary<string, object>();
    }
}