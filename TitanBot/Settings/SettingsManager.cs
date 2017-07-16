using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Storage;
using TitanBot.Dependencies;
using TitanBot.Formatting;
using TitanBot.Util;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private IDatabase Database { get; }
        private IDependencyFactory DependencyFactory { get; }
        private ITextResourceManager TextResourceManager { get; }
        
        public IReadOnlyList<IEditableSettingGroup> GuildSettingGroups => GuildGroups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> GuildGroups { get; } = new Dictionary<Type, IEditableSettingGroup>();

        public IReadOnlyList<IEditableSettingGroup> GlobalSettingGroups => GlobalGroups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> GlobalGroups { get; } = new Dictionary<Type, IEditableSettingGroup>();

        public IReadOnlyList<IEditableSettingGroup> UserSettingGroups => UserGroups.Select(g => g.Value).ToList().AsReadOnly();
        private Dictionary<Type, IEditableSettingGroup> UserGroups { get; } = new Dictionary<Type, IEditableSettingGroup>();

        private Dictionary<(Type, ulong), object> Cached { get; } = new Dictionary<(Type, ulong), object>();

        internal SettingsManager(IDatabase database, ITextResourceManager textManager, IDependencyFactory factory)
        {
            Database = database;
            DependencyFactory = factory;
            TextResourceManager = textManager;

            AddGuildSetting<GeneralGuildSetting>().WithName("General")
                                                  .WithDescription("SETTINGS_GUILD_GENERAL_DESCRIPTION")
                                                  .AddSetting(s => s.Prefix)
                                                  .AddSetting(s => s.PermOverride)
                                                  .AddSetting(s => s.RoleOverride, (ICommandContext c, IRole[] roles) => roles.Select(r => r.Id).ToArray(), viewer: (c, r) => string.Join(", ", r?.Select(id => $"<@&{id}>")))
                                                  .AddSetting(s => s.DateTimeFormat)
                                                  .AddSetting(s => s.PreferredLanguage, validator: (c, v) => TextResourceManager.GetLanguageCoverage(v) > 0 ? null : "LOCALE_UNKNOWN")
                                                  .WithNotes("SETTINGS_GUILD_GENERAL_NOTES")
                                                  .Finalise();

            AddGlobalSetting<GeneralGlobalSetting>().WithName("General")
                                                    .WithDescription("SETTINGS_GLOBAL_GENERAL_DESCRIPTION")
                                                    .AddSetting(s => s.DefaultPrefix)
                                                    .AddSetting(s => s.Owners, (ICommandContext c, IUser[] u) => u.Select(p => p.Id).ToArray(), viewer: (c, u) => string.Join(", ", u.Select(p => $"<@{p}>")))
                                                    .Finalise();

            AddUserSetting<GeneralUserSetting>().WithName("General")
                                                .WithDescription("SETTINGS_USER_GENERAL_DESCRIPTION")
                                                .AddSetting(s => s.Language, validator: (c, v) => TextResourceManager.GetLanguageCoverage(v) > 0 ? null : "LOCALE_UNKNOWN")
                                                .AddSetting(s => s.FormatType, validator: (c, v) => v == FormattingType.DEFAULT || c.Formatter.AcceptedFormats.Contains(v) ? null : "FORMATTINGTYPE_UNKNOWN", viewer: (c, f) => c.Formatter.GetName(f))
                                                .AddSetting(s => s.UseEmbeds)
                                                .Finalise();
        }

        private T GetSetting<T>(ulong id)
        {
            if (Cached.TryGetValue((typeof(T), id), out object cached))
                return (T)cached;

            var record = Database.FindById<Setting>(id).Result ?? new Setting { Serialized = "{}", Id = id };
            var targetType = typeof(T).FullName;
            var settings = JObject.Parse(record.Serialized);

            T obj;

            if (settings.TryGetValue(typeof(T).ToString(), out JToken setting))
                obj = setting.ToObject<T>();
            obj = JsonConvert.DeserializeObject<T>("{}");

            Cached[(typeof(T), id)] = obj;
            return obj;
        }

        private void SaveSetting<T>(ulong id, T setting)
        {
            var record = Database.FindById<Setting>(id).Result ?? new Setting { Serialized = "{}", Id = id };
            var targetType = typeof(T).FullName;
            var settings = JObject.Parse(record.Serialized);

            settings[targetType] = JObject.FromObject(setting);
            record.Serialized = JsonConvert.SerializeObject(settings);
            
            Database.Upsert(record).Wait();
            Cached[(typeof(T), id)] = setting;
        }

        public async void ResetSettings(ulong guildId)
        {
            await Database.Delete<Setting>(s => s.Id == guildId);
        }

        public T GetGuildGroup<T>(ulong id)
            => GetSetting<T>(id);
        public T GetGlobalGroup<T>()
            => GetSetting<T>(1);
        public T GetUserGroup<T>(ulong id)
            => GetSetting<T>(id);

        public void SaveGuildGroup<T>(ulong id, T setting)
            => SaveSetting(id, setting);
        public void SaveGlobalGroup<T>(T setting)
            => SaveSetting(1, setting);
        public void SaveUserGroup<T>(ulong id, T setting)
            => SaveSetting(id, setting);

        public void EditGuildGroup<T>(ulong id, Action<T> changes)
            => SaveGuildGroup(id, MiscUtil.InlineAction(GetGuildGroup<T>(id), changes));
        public void EditGlobalGroup<T>(Action<T> changes)
            => SaveGlobalGroup(MiscUtil.InlineAction(GetGlobalGroup<T>(), changes));
        public void EditUserGroup<T>(ulong id, Action<T> changes)
            => SaveUserGroup(id, MiscUtil.InlineAction(GetUserGroup<T>(id), changes));

        public IEditableSettingBuilder<T> AddGuildSetting<T>()
            => AddGuildSetting(GetGuildGroup<T>, SaveGuildGroup);
        public IEditableSettingBuilder<T> AddGlobalSetting<T>()
            => AddGlobalSetting(GetGlobalGroup<T>, SaveGlobalGroup);
        public IEditableSettingBuilder<T> AddUserSetting<T>()
            => AddUserSetting(GetUserGroup<T>, SaveUserGroup);

        public IEditableSettingBuilder<T> AddGuildSetting<T>(Func<ulong, T> retriever, Action<ulong, T> saver)
            => new EditableSettingBuilder<T>(GuildGroups, DependencyFactory, retriever, saver);
        public IEditableSettingBuilder<T> AddGlobalSetting<T>(Func<T> retriever, Action<T> saver)
            => new EditableSettingBuilder<T>(GlobalGroups, DependencyFactory, i => retriever(), (i, v) => saver(v));
        public IEditableSettingBuilder<T> AddUserSetting<T>(Func<ulong, T> retriever, Action<ulong, T> saver)
            => new EditableSettingBuilder<T>(UserGroups, DependencyFactory, retriever, saver);
    }
}
