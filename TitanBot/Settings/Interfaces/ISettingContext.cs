using System;

namespace TitanBot.Settings
{
    public interface ISettingContext
    {
        ulong Id { get; }
        T Get<T>();
        void Edit<T>(Action<T> edits);
        void ResetAll();
    }
}
