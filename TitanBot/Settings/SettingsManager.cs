using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Storage;
using TitanBot.Dependencies;

namespace TitanBot.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }
        private IDependencyFactory DependencyFactory { get; }

        public GlobalSetting GlobalSettings { get; }
        
        public IReadOnlyList<IEditableSettingGroup> EditableSettingGroups => Groups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> Groups { get; } = new Dictionary<Type, IEditableSettingGroup>();

        public IReadOnlyList<IEditableSettingGroup> EditableGlobalSettingsGroups => GlobalGroups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> GlobalGroups { get; } = new Dictionary<Type, IEditableSettingGroup>();

        public T GetCustomGlobal<T>()
            => GlobalSettings.GetCustom<T>();
        public void SaveCustomGlobal<T>(T setting)
            => GlobalSettings.SaveCustom(setting);

        internal SettingsManager(IDatabase database, IDependencyFactory factory)
        {
            Database = database;
            DependencyFactory = factory;
            GlobalSettings = new GlobalSetting(this, database);

            Register<GeneralSettings>().WithName("General")
                                       .WithDescription("General settings for the bot")
                                       .AddSetting(s => s.Prefix)
                                       .AddSetting(s => s.PermOverride)
                                       .AddSetting(s => s.RoleOverride, (IRole[] roles) => roles.Select(r => r.Id).ToArray(), viewer: r => string.Join(", ", r?.Select(id => $"<@&{id}>")))
                                       .AddSetting(s => s.DateTimeFormat)
                                       .Finalise();

            RegisterGlobal((m, id) => m.GlobalSettings, (m, id, o) => { }).WithName("General")
                                                                          .WithDescription("General global settings")
                                                                          .AddSetting(s => s.DefaultPrefix)
                                                                          .AddSetting(s => s.Owners, (IUser[] u) => u.Select(p => p.Id).ToArray(), viewer: u => string.Join(", ", u.Select(p => $"<@{p}>")))
                                                                          .Finalise();
        }

        public T GetGroup<T>(ulong guildId)
        {
            var targetType = typeof(T).FullName;
            var settings = JObject.Parse(Database.FindOne<Setting>(s => s.Id == guildId).Result?.Serialized ?? "{}");

            if (settings.TryGetValue(typeof(T).ToString(), out JToken setting))
                return setting.ToObject<T>();
            return JsonConvert.DeserializeObject<T>("{}");
        }

        public void SaveGroup<T>(ulong guildId, T settings)
        {
            var current = Database.FindOne<Setting>(s => s.Id == guildId).Result ?? new Setting { Id = guildId, Serialized = "{}" };

            var settingsObj = JObject.Parse(current.Serialized);

            settingsObj[typeof(T).ToString()] = JObject.FromObject(settings);

            Database.Upsert(new Setting
            {
                Id = guildId,
                Serialized = settingsObj.ToString()
            }).Wait();
        }

        public IEditableSettingBuilder<T> Register<T>()
            => Register((m, id) => m.GetGroup<T>(id), (m, id, o) => m.SaveGroup(id, o));

        public IEditableSettingBuilder<T> RegisterGlobal<T>()
            => RegisterGlobal((m, id) => m.GetCustomGlobal<T>(), (m, id, o) => m.SaveCustomGlobal(o));

        public IEditableSettingBuilder<T> Register<T>(Func<ISettingsManager, ulong, T> retriever, Action<ISettingsManager, ulong, T> saver)
            => DependencyFactory.WithInstance(Groups).WithInstance(retriever).WithInstance(saver).Construct<IEditableSettingBuilder<T>>();

        public IEditableSettingBuilder<T> RegisterGlobal<T>(Func<ISettingsManager, ulong, T> retriever, Action<ISettingsManager, ulong, T> saver)
            => DependencyFactory.WithInstance(GlobalGroups).WithInstance(retriever).WithInstance(saver).Construct<IEditableSettingBuilder<T>>();
    }
}
