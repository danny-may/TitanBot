using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBotBase.Database;
using TitanBotBase.Dependencies;

namespace TitanBotBase.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }
        private IDependencyFactory DependencyFactory { get; }

        public GlobalSetting GlobalSettings { get; }
        
        public IReadOnlyList<IEditableSettingGroup> EditableSettingGroups => _groups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> _groups { get; } = new Dictionary<Type, IEditableSettingGroup>();

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
                                       .Finalise();
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

        public IEditableSettingBuilder<T> Register<T>()
            => DependencyFactory.WithInstance(_groups).Construct<IEditableSettingBuilder<T>>();
    }
}
