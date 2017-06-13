namespace TitanBotBase.Settings
{
    public interface ISettingsManager
    {
        GlobalSetting GetGlobalSettings();
        T GetSettingGroup<T>(ulong id) where T : ISettingGroup, new();
        void SaveSettingsGroup<T>(T settings) where T : ISettingGroup, new();
    }
}