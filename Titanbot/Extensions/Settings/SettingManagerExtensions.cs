using Titanbot.Extensions.Settings.Interfaces;
using Titanbot.Settings.Interfaces;

namespace Titanbot.Extensions.Settings
{
    public static class SettingManagerExtensions
    {
        public static ISettingEditor<TSetting> GetOrCreateEditor<TSetting>(this ISettingManager manager)
            where TSetting : class, new()
            => SettingEditorManager.GetFor(manager).GetOrCreateEditor<TSetting>();
    }
}