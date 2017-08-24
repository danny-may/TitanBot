using Discord;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TitanBot.Dependencies;
using TitanBot.Helpers;
using TitanBot.Storage;

namespace TitanBot.Settings
{
    class SettingManager : ISettingManager
    {
        private class GlobalEntity : IEntity<ulong>
        {
            public ulong Id => 1;
        }

        public IEntity<ulong> Global { get; }

        private IDatabase Database { get; }
        private IDependencyFactory Factory { get; }
        private Dictionary<SettingScope, Dictionary<Type, ISettingEditorCollection>> _settingEditors { get; } = new Dictionary<SettingScope, Dictionary<Type, ISettingEditorCollection>>();
        private CachedDictionary<ulong, SettingContext> CachedContexts { get; }
        private CachedDictionary<ulong, GroupingMap> CachedGroups { get; }

        public SettingManager(IDatabase database, IDependencyFactory factory)
        {
            Global = new GlobalEntity();
            Database = database;
            Factory = factory;
            CachedContexts = CachedDictionary.FromSource((ulong e) => new SettingContext(Database, e));
            CachedGroups = CachedDictionary.FromSource((ulong e) => Database.FindById<GroupingMap>(e).Result ?? new GroupingMap { Id = e });
        }

        public ISettingContext GetContext(IEntity<ulong> entity)
        {
            if (entity == null)
                return null;
            return GetContext(entity.Id);
        }

        public ISettingContext GetContext(ulong entity)
            => CachedContexts[entity];

        public ISettingEditorCollection<T> GetEditorCollection<T>(SettingScope scope)
        {
            if (!_settingEditors.TryGetValue(scope, out var dict) || dict == null)
                _settingEditors[scope] = new Dictionary<Type, ISettingEditorCollection>();

            if (!_settingEditors[scope].TryGetValue(typeof(T), out var x) || x is ISettingEditorCollection<T> collection)
                _settingEditors[scope][typeof(T)] = Factory.Construct<ISettingEditorCollection<T>>();

            return _settingEditors[scope][typeof(T)] as ISettingEditorCollection<T>;
        }

        public IReadOnlyList<ISettingEditorCollection> GetEditors(SettingScope scope)
        {
            if (!_settingEditors.TryGetValue(scope, out var dict) || dict == null)
                _settingEditors[scope] = new Dictionary<Type, ISettingEditorCollection>();

            return _settingEditors[scope].Values.ToList().AsReadOnly();
        }

        public void ResetSettings(IEntity<ulong> entity)
        {
            if (entity != null)
                ResetSettings(entity.Id);
        }

        public void ResetSettings(ulong entity)
            => GetContext(entity).ResetAll();

        public async void Migrate(Dictionary<string, Type> typeMap)
        {
            var records = await Database.Find<Setting>(r => true);
            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.Serialized))
                    continue;
                var parsed = JObject.Parse(record.Serialized);

                var removals = new List<string>();

                foreach (var entry in parsed)
                {
                    if (!typeMap.TryGetValue(entry.Key, out var type))
                        continue;
                    var obj = entry.Value.ToObject(type);
                    record.Settings[type.Name] = obj;
                    removals.Add(entry.Key);
                }

                removals.ForEach(r => parsed.Remove(r));

                if (parsed.Count == 0)
                    record.Serialized = "";
                else
                    record.Serialized = parsed.ToString();
            }
            await Database.Upsert(records);
        }


        public IReadOnlyDictionary<int, string[]> GetGroups(IEntity<ulong> entity)
            => entity == null ? null : GetGroups(entity.Id);
        public IReadOnlyDictionary<int, string[]> GetGroups(ulong entity)
            => CachedGroups[entity].Mapping.ToImmutableDictionary();

        public void SetGroup(IEntity<ulong> entity, int value, string[] keys)
        {
            if (entity != null)
                SetGroup(entity.Id, value, keys);
        }
        public void SetGroup(ulong entity, int value, string[] keys)
        {
            var record = CachedGroups[entity] ?? new GroupingMap { Id = entity };
            record.Mapping[value] = keys.Select(k => k.ToLower()).Distinct().ToArray();
            Database.Upsert(record).Wait();
        }

        public void RemoveGroup(IEntity<ulong> entity, int value)
        {
            if (entity != null)
                RemoveGroup(entity.Id, value);
        }
        public void RemoveGroup(ulong entity, int value)
        {
            var record = CachedGroups[entity] ?? new GroupingMap { Id = entity };
            record.Mapping.Remove(value);
            Database.Upsert(record).Wait();
        }
    }
}
