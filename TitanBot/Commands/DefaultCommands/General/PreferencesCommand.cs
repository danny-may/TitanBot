using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.General
{
    [Description(Desc.PREFERENCES)]
    [Alias("Pref", "Preference")]
    class PreferencesCommand : SettingCommand
    {
        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.User);
        protected override IEntity<ulong> SettingContext => Author;

        public PreferencesCommand(ITypeReaderCollection readers)
            : base(readers) { }

        [Call]
        [Usage(Usage.PREFERENCES_DEFAULT)]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Toggle")]
        [Usage(Usage.PREFERENCES_TOGGLE)]
        new Task ToggleSettingAsync(string key)
            => base.ToggleSettingAsync(key);

        [Call("Set")]
        [Usage(Usage.PREFERENCES_SET)]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
