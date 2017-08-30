using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private string GetKey<T>(int group)
            => group == 0 ? typeof(T).Name : typeof(T).Name + "_Group:" + group;

        public void Edit<T>(Action<T> edits)
            => Edit(0, edits);
        public void Edit<T>(int group, Action<T> edits)
        {
            var setting = Get<T>(group);
            edits(setting);

            Record.Settings[GetKey<T>(group)] = setting;
            Database.Upsert(Record).Wait();
        }

        public T Get<T>()
            => Get<T>(0);
        public T Get<T>(int group)
        {
            if (Record.Settings.TryGetValue(GetKey<T>(group), out object obj) && obj != null)
                return (T)obj;
            return JsonConvert.DeserializeObject<T>("{}");
        }

        public void ResetAll()
        {
            Record.Settings = new Dictionary<string, object>();
            Database.Upsert(Record).Wait();
        }
    }
}
