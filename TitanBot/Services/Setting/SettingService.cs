using Discord;
using System.Collections.Concurrent;
using TitanBot.Core.Services.Database;
using TitanBot.Core.Services.Setting;

namespace TitanBot.Services.Setting
{
    public class SettingService : ISettingService
    {
        #region Fields

        private readonly IDatabaseService _database;
        private ConcurrentDictionary<ulong, SettingCollection> _collections = new ConcurrentDictionary<ulong, SettingCollection>();

        #endregion Fields

        #region Constructors

        public SettingService(IDatabaseService database)
        {
            _database = database;
        }

        #endregion Constructors

        #region ISettingService

        public void DestroyContext(ulong id)
        {
            if (_collections.TryRemove(id, out var coll))
                coll.Destroy();
        }

        public void DestroyContext(IEntity<ulong> entity)
            => DestroyContext(entity.Id);

        public void DestroyContext(ISettingCollection context)
            => DestroyContext(context.ContextId);

        public ISettingCollection GetContext(ulong id)
            => _collections.GetOrAdd(id, new SettingCollection(_database, id));

        public ISettingCollection GetContext(IEntity<ulong> entity)
            => GetContext(entity.Id);

        public ISettingCollection GetGlobalContext()
            => GetContext(1);

        #endregion ISettingService
    }
}