using System.Collections.Generic;
using System.Reflection;

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
        IReadOnlyList<IEditableSettingGroup> EditableSettingGroups { get; }
    }
}