using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;

namespace TitanBot.Commands.DefaultCommands.General
{
    [Description("PREFERENCES_HELP_DESCRIPTION")]
    [Alias("Pref", "Preference")]
    class PreferencesCommand : SettingCommand
    {
        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.User);
        protected override IEntity<ulong> SettingContext => Author;

        public PreferencesCommand(ITypeReaderCollection readers)
            : base(readers) { }

        [Call]
        [Usage("PREFERENCES_HELP_USAGE_DEFAULT")]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Toggle")]
        [Usage("PREFERENCES_HELP_USAGE_TOGGLE")]
        new Task ToggleSettingAsync(string key)
            => base.ToggleSettingAsync(key);

        [Call("Set")]
        [Usage("PREFERENCES_HELP_USAGE_SET")]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
