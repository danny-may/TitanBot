using System.Threading.Tasks;

namespace TitanBotBase.Commands.DefaultCommands.Owner
{
    class Test : Command
    {
        [Call]
        async Task TestAsync()
        {
            var custom = GlobalSettings.GetCustom<GlobalSetting>();
            var oldVal = custom.Test;
            custom.Test = GetHashCode();
            GlobalSettings.SaveCustom(custom);
            GlobalSettings.DefaultPrefix = GlobalSettings.DefaultPrefix;
            custom = GlobalSettings.GetCustom<GlobalSetting>();
            await ReplyAsync($"Global settings\nOld val: `{oldVal}`\nNew val: `{custom.Test}`");

            var setting = SettingsManager.GetGroup<CustomSetting>(Guild.Id);
            oldVal = setting.Test;
            setting.Test = setting.GetHashCode();
            SettingsManager.SaveGroup(Guild.Id, setting);
            setting = SettingsManager.GetGroup<CustomSetting>(Guild.Id);
            await ReplyAsync($"Custom settings\nOld val: `{oldVal}`\nNew val: `{setting.Test}`");
        }

        private class GlobalSetting
        {
            public int Test { get; set; } = 0;
        }

        private class CustomSetting
        {
            public int Test { get; set; } = 0;
        }
    }
}
