using System;

namespace Titanbot.Settings.Interfaces
{
    public interface ISettingCollection
    {
        decimal ContextId { get; }

        bool TryGet<TSetting>(out TSetting setting) where TSetting : class, new();
        TSetting GetOrNew<TSetting>() where TSetting : class, new();

        void Set<TSetting>(TSetting setting) where TSetting : class, new();
        void Update<TSetting>(Func<TSetting, TSetting> editor) where TSetting : class, new();

        void Delete<TSetting>() where TSetting : class, new();
        void Reset<TSetting>() where TSetting : class, new();

        void Clear();
    }
}