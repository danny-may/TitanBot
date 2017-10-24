using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TitanBot.Core.Models;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Setting;

namespace TitanBot.Services.Setting
{
    public class SettingCollection : ISettingCollection
    {
        #region Fields

        private readonly IDatabaseService _database;
        private readonly Dictionary<Type, object> _models = new Dictionary<Type, object>();
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private readonly List<BoundModel> _boundTo = new List<BoundModel>();

        private SettingRecord _settingRecord;
        private Dictionary<string, string> _settings => _settingRecord.Settings;

        private object _bindingLock = new object();
        private object _valueLock = new object();
        private object _modelLock = new object();

        #endregion Fields

        #region Constructors

        public SettingCollection(IDatabaseService database, ulong id)
        {
            _database = database;
            ContextId = id;

            _settingRecord = _database.Query<SettingRecord>(t => t.GetOrAdd(ContextId, i => new SettingRecord { Id = (ulong)i }));
        }

        #endregion Constructors

        #region Methods

        private void PropertyChanged(object sender, BindingUpdateEventArgs e)
            => Save(e.PropertyName, e.NewValue);

        internal void Destroy()
        {
            _database.Query<SettingRecord>(t => t.Delete(ContextId));
            _settingRecord = null;
        }

        #endregion Methods

        #region ISettingCollection

        public ulong ContextId { get; }

        public T GetModel<T>() where T : new()
        {
            lock (_modelLock)
            {
                var type = typeof(T);
                if (!_models.TryGetValue(type, out var model) || !(model is T))
                {
                    model = new T();
                    foreach (var item in _settings)
                    {
                        var property = type.GetProperty(item.Key);
                        var value = JsonConvert.DeserializeObject(item.Value, property.PropertyType);
                        property.SetValue(model, value);
                    }

                    _models[type] = model;
                }

                if (model is BoundModel bound)
                    BindTo(bound);

                return (T)model;
            }
        }

        public void SaveModel<T>(T model) where T : new()
        {
            lock (_modelLock)
            {
                var type = typeof(T);
                _models[type] = model;
                foreach (var property in type.GetProperties())
                    _settings[property.Name] = JsonConvert.SerializeObject(property.GetValue(model));
                _database.Query<SettingRecord>(t => t.Upsert(_settingRecord));
            }
        }

        public void EditModel<T>(Action<T> edit) where T : new()
        {
            var model = GetModel<T>();
            edit(model);
            SaveModel(model);
        }

        public T Get<T>(string key)
        {
            lock (_valueLock)
            {
                if (!_values.TryGetValue(key, out var value) || !(value is T))
                {
                    var serialised = _settings.TryGetValue(key, out var s) ? s : "{}";

                    value = JsonConvert.DeserializeObject<T>(serialised);

                    _values[key] = value;
                }

                if (value is BoundModel bound)
                    BindTo(bound);

                return (T)value;
            }
        }

        public void Save<T>(string key, T value)
        {
            lock (_valueLock)
            {
                _values[key] = value;
                _settings[key] = JsonConvert.SerializeObject(value);
                _database.Query<SettingRecord>(t => t.Upsert(_settingRecord));
            }
        }

        public void Edit<T>(string key, Action<T> edit) where T : new()
        {
            var value = Get<T>(key);
            edit(value);
            Save(key, value);
        }

        public void BindTo<T>(T model) where T : BoundModel
        {
            lock (_bindingLock)
            {
                if (_boundTo.Contains(model))
                    return;
                _boundTo.Add(model);
                model.BindingUpdated += PropertyChanged;
            }
        }

        public void UnBind<T>(T model) where T : BoundModel
        {
            lock (_bindingLock)
            {
                if (!_boundTo.Contains(model))
                    return;
                _boundTo.Remove(model);
                model.BindingUpdated -= PropertyChanged;
            }
        }

        #endregion ISettingCollection
    }
}