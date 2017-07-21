using Newtonsoft.Json;
using System;
using TitanBot.Storage;

namespace TitanBot.Settings
{
    class SettingContext : ISettingContext
    {
        private IDatabase Database { get; }
        private Setting Record { get; }
        public ulong Id { get; }

        public SettingContext(IDatabase database, ulong id)
        {
            Database = database;
            Id = id;

            Record = Database.FindById<Setting>(id).Result ?? new Setting { Id = id };
        }

        public void Edit<T>(Action<T> edits)
        {
            var setting = Get<T>();
            edits(setting);

            Record.Settings[typeof(T).Name] = setting;
            Database.Upsert(Record).Wait();
        }

        public T Get<T>()
        {
            if (Record.Settings.TryGetValue(typeof(T).Name, out object obj) && obj != null)
                return (T)obj;
            return JsonConvert.DeserializeObject<T>("{}");
        }
    }
}
