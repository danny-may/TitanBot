using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description(Desc.SETTINGS)]
    [DefaultPermission(8)]
    [Alias("Setting")]
    [RequireContext(ContextType.Guild)]
    public class SettingsCommand : SettingCommand
    {
        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.Guild);
        protected override IEntity<ulong> SettingContext => Guild;

        public SettingsCommand(ITypeReaderCollection readers)
            : base(readers) { }

        [Call]
        [Usage(Usage.SETTINGS_DEFAULT)]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Toggle")]
        [Usage(Usage.SETTINGS_TOGGLE)]
        new Task ToggleSettingAsync(string key)
            => base.ToggleSettingAsync(key);

        [Call("Set")]
        [Usage(Usage.SETTINGS_SET)]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
