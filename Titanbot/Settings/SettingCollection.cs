using System;
using Titanbot.Settings.Interfaces;
using Titanbot.Settings.Models;
using Titansmasher.Extensions;
using Titansmasher.Services.Database.Interfaces;

namespace Titanbot.Settings
{
    public class SettingCollection : ISettingCollection
    {
        #region Fields

        private readonly IDatabaseTable<SettingCollectionRecord> _table;

        private SettingCollectionRecord _record
            => _table.Find(ContextId);

        #endregion Fields

        #region Constructors

        public SettingCollection(IDatabaseTable<SettingCollectionRecord> table, decimal id)
        {
            ContextId = id;
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        #endregion Constructors

        #region Methods

        private string GetKey<TSetting>()
            => typeof(TSetting).FullName;

        private void Update(Action<SettingCollectionRecord> updater)
        {
            var record = _record;
            updater(record);
            _table.Upsert(record);
        }

        #endregion Methods

        #region ISettingCollection

        public decimal ContextId { get; }

        public void Clear()
            => _table.Delete(ContextId);

        public void Delete<TSetting>() where TSetting : class, new()
            => _record.Settings.Remove(GetKey<TSetting>());

        public TSetting GetOrNew<TSetting>() where TSetting : class, new()
            => TryGet<TSetting>(out var setting) ? setting : new TSetting();

        public void Reset<TSetting>() where TSetting : class, new()
            => Update(r => r.Settings[GetKey<TSetting>()] = new TSetting());

        public void Set<TSetting>(TSetting setting) where TSetting : class, new()
            => Update(r => r.Settings[GetKey<TSetting>()] = setting ?? new TSetting());

        public bool TryGet<TSetting>(out TSetting setting) where TSetting : class, new()
            => _record.Settings.TryGetCast(GetKey<TSetting>(), out setting);

        public void Update<TSetting>(Func<TSetting, TSetting> editor) where TSetting : class, new()
            => Update(r => (editor ?? throw new ArgumentNullException(nameof(editor)))(GetOrNew<TSetting>()));

        #endregion ISettingCollection
    }
}