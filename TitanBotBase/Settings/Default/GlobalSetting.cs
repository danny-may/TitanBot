using Newtonsoft.Json.Linq;
using System;
using TitanBotBase.Database;

namespace TitanBotBase.Settings
{
    public class GlobalSetting
    {
        private SettingsManager Manager { get; }
        private IDatabase Database { get; }
        private GlobalSettingRecord Record { get; set; }
        private object _lock = new object();

        internal GlobalSetting(SettingsManager manager, IDatabase database)
        {
            Manager = manager;
            Database = database;
            Record = Database.AddOrGet(0, () => new GlobalSettingRecord()).Result;
        }



        T GetCustom<T>()
            => AdditionalSettings.ToObject<T>();
        void SaveCustom<T>(T obj)
            => AdditionalSettings = JObject.FromObject(obj);

        public string DefaultPrefix
        {
            get => Record.DefaultPrefix;
            set => ModifySafe(s => s.DefaultPrefix = value);
        }
        public string Token
        {
            get => Record.Token;
            set => ModifySafe(s => s.Token = value);
        }
        public ulong[] Owners
        {
            get => Record.Owners;
            set => ModifySafe(s => s.Owners = value);
        }
        public JObject AdditionalSettings
        {
            get => Record.AdditionalSettings;
            set => ModifySafe(s => s.AdditionalSettings = value);
        }

        private void ModifySafe(Action<GlobalSettingRecord> edit)
        {
            lock (_lock)
            {
                edit(Record);
                Record = Database.GetUpsert(Record).Result;
            }
        }

        private class GlobalSettingRecord : IDbRecord
        {
            public ulong Id { get; set; } = 0;
            public string DefaultPrefix { get; set; } = "t$";
            public string Token { get; set; } = "";
            public ulong[] Owners { get; set; } = new ulong[0];
            public JObject AdditionalSettings { get; set; } = new JObject();
        }
    }
}
