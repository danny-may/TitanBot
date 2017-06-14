using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }

        public GlobalSetting GlobalSettings { get; }

        public T GetCustomGlobal<T>()
            => GlobalSettings.GetCustom<T>();
        public void SaveCustomGlobal<T>(T setting)
            => GlobalSettings.SaveCustom(setting);

        internal SettingsManager(IDatabase database)
        {
            Database = database;
            GlobalSettings = new GlobalSetting(this, database);
        }

        public T GetGroup<T>(ulong guildId)
        {
            var targetType = typeof(T).FullName;
            var setting = Database.FindOne<Setting>(s => s.Id == guildId && s.Type == targetType).Result?.Serialized ?? "{}";
            return JsonConvert.DeserializeObject<T>(setting);
        }

        public void SaveGroup<T>(ulong guildId, T settings)
        {
            Database.Upsert(new Setting
            {
                Id = guildId,
                Type = typeof(T).FullName,
                Serialized = JsonConvert.SerializeObject(settings)
            }).Wait();
        }
    }
}
