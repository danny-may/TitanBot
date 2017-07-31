using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(TitanBotResource.GLOBALSETTINGS_HELP_DESCRIPTION)]
    [Alias("GSettings", "GSetting")]
    [RequireOwner]
    public class GlobalSettingsCommand : SettingCommand
    {
        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.Global);
        protected override IEntity<ulong> SettingContext => SettingsManager.Global;

        public GlobalSettingsCommand(ITypeReaderCollection readers)
            : base(readers) { }

        [Call]
        [Usage(TitanBotResource.GLOBALSETTINGS_HELP_USAGE_DEFAULT)]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Toggle")]
        [Usage(TitanBotResource.GLOBALSETTINGS_HELP_USAGE_TOGGLE)]
        new Task ToggleSettingAsync(string key)
            => base.ToggleSettingAsync(key);

        [Call("Set")]
        [Usage(TitanBotResource.GLOBALSETTINGS_HELP_USAGE_SET)]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
