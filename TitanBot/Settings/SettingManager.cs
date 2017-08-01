﻿using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Dependencies;
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
        private Dictionary<ulong, ISettingContext> Cached { get; } = new Dictionary<ulong, ISettingContext>();

        public SettingManager(IDatabase database, IDependencyFactory factory)
        {
            Global = new GlobalEntity();
            Database = database;
            Factory = factory;
        }

        public ISettingContext GetContext(IEntity<ulong> entity)
        {
            if (entity == null)
                return null;
            return GetContext(entity.Id);
        }

        public ISettingContext GetContext(ulong entity)
        {
            if (!Cached.ContainsKey(entity))
                Cached[entity] = new SettingContext(Database, entity);
            return Cached[entity];
        }

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
    }
}
