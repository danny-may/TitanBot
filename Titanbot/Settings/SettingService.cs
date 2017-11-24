using Discord;
using System;
using System.Collections.Concurrent;
using Titanbot.Settings.Interfaces;
using Titanbot.Settings.Models;
using Titansmasher.Services.Database.Interfaces;

namespace Titanbot.Settings
{
    public class SettingService : ISettingService
    {
        #region Fields

        private readonly IDatabaseService _db;
        private readonly ConcurrentDictionary<decimal, ISettingCollection> _cached = new ConcurrentDictionary<decimal, ISettingCollection>();

        #endregion Fields

        #region Constructors

        public SettingService(IDatabaseService database)
        {
            _db = database ?? throw new ArgumentNullException(nameof(database));
        }

        #endregion Constructors

        #region ISettingManager

        public ISettingCollection this[decimal id] => CollectionFor(id);

        public ISettingCollection this[IEntity<decimal> id] => CollectionFor(id);

        public ISettingCollection Global => CollectionFor(1);

        public ISettingCollection CollectionFor(decimal id)
            => _cached.GetOrAdd(id, i => new SettingCollection(_db.GetTable<SettingCollectionRecord>(), i));

        public ISettingCollection CollectionFor(IEntity<decimal> id)
            => CollectionFor(id?.Id ?? throw new ArgumentNullException(nameof(id)));

        #endregion ISettingManager
    }
}