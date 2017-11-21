using Titanbot.Settings.Interfaces;

namespace Titanbot.Extensions.Settings.Interfaces
{
    public interface ISettingEditorManager
    {
        ISettingManager Setttings { get; }

        ISettingEditor<TSetting> GetOrCreateEditor<TSetting>()
            where TSetting : class, new();
    }
}