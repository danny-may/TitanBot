using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }

        internal SettingsManager(IDatabase database)
        {
            Database = database;
        }

        public T GetSettingGroup<T>(ulong id)
            where T : ISettingGroup, new()
        {
            return Database.AddOrGet(id, () => new T { Id = id }).Result;
        }

        public GlobalSetting GetGlobalSettings()
            => Database.AddOrGet(0, () => new GlobalSetting { Id = 0 }).Result;

        public void SaveSettingsGroup<T>(T settings) 
            where T : ISettingGroup, new()
        {
            Database.Upsert(settings);
        }
    }
}
