using System;
using System.Reflection;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }

        public GlobalSetting GlobalSettings { get; }

        internal SettingsManager(IDatabase database)
        {
            Database = database;
            GlobalSettings = new GlobalSetting(this, database);
        }

        public T GetGroup<T>(ulong id)
            where T : ISettingGroup, new()
        {
            return Database.AddOrGet(id, () => new T { Id = id }).Result;
        }

        public void SaveGroup<T>(T settings) 
            where T : ISettingGroup, new()
        {
            Database.Upsert(settings);
        }

        public void Install(Assembly assembly)
        {
            
        }
    }
}
