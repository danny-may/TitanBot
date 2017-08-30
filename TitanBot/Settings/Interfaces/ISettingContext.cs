using System;

namespace TitanBot.Settings
{
    public interface ISettingContext
    {
        ulong Id { get; }
        T Get<T>();
        T Get<T>(int group);
        void Edit<T>(Action<T> edits);
        void Edit<T>(int group, Action<T> edits);
        void ResetAll();
    }
}
