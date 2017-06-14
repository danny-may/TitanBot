using System.Reflection;

namespace TitanBotBase.Settings
{
    public interface ISettingsManager
    {
        GlobalSetting GlobalSettings { get; }
        T GetGroup<T>(ulong id) where T : ISettingGroup, new();
        void SaveGroup<T>(T settings) where T : ISettingGroup, new();
        void Install(Assembly assembly);
    }
}