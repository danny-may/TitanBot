using System;
using System.Collections.Generic;

namespace TitanBotBase.Settings
{
    public interface ISettingsManager
    {
        GlobalSetting GlobalSettings { get; }
        T GetGroup<T>(ulong id);
        void SaveGroup<T>(ulong id, T settings);
        T GetCustomGlobal<T>();
        void SaveCustomGlobal<T>(T setting);
        IEditableSettingBuilder<T> Register<T>();
        IEditableSettingBuilder<T> RegisterGlobal<T>();
        IEditableSettingBuilder<T> Register<T>(Func<ISettingsManager, ulong, T> retriever, Action<ISettingsManager, ulong, T> saver);
        IEditableSettingBuilder<T> RegisterGlobal<T>(Func<ISettingsManager, ulong, T> retriever, Action<ISettingsManager, ulong, T> saver);
        IReadOnlyList<IEditableSettingGroup> EditableSettingGroups { get; }
        IReadOnlyList<IEditableSettingGroup> EditableGlobalSettingsGroups { get; }
    }
}