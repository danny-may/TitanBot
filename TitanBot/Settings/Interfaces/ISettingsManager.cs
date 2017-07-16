using System;
using System.Collections.Generic;

namespace TitanBot.Settings
{
    public interface ISettingsManager
    {
        T GetGuildGroup<T>(ulong id);
        T GetGlobalGroup<T>();
        T GetUserGroup<T>(ulong id);
        void SaveGuildGroup<T>(ulong id, T setting);
        void SaveGlobalGroup<T>(T setting);
        void SaveUserGroup<T>(ulong id, T setting);
        void EditGuildGroup<T>(ulong id, Action<T> changes);
        void EditGlobalGroup<T>(Action<T> changes);
        void EditUserGroup<T>(ulong id, Action<T> changes);
        IEditableSettingBuilder<T> AddGuildSetting<T>();
        IEditableSettingBuilder<T> AddGlobalSetting<T>();
        IEditableSettingBuilder<T> AddUserSetting<T>();
        IEditableSettingBuilder<T> AddGuildSetting<T>(Func<ulong, T> retriever, Action<ulong, T> saver);
        IEditableSettingBuilder<T> AddGlobalSetting<T>(Func<T> retriever, Action<T> saver);
        IEditableSettingBuilder<T> AddUserSetting<T>(Func<ulong, T> retriever, Action<ulong, T> saver);
        IReadOnlyList<IEditableSettingGroup> GuildSettingGroups { get; }
        IReadOnlyList<IEditableSettingGroup> GlobalSettingGroups { get; }
        IReadOnlyList<IEditableSettingGroup> UserSettingGroups { get; }
        void ResetSettings(ulong id);
    }
}