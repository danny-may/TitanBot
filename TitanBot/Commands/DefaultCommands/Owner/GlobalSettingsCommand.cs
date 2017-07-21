using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("GLOBALSETTINGS_HELP_DESCRIPTION")]
    [Alias("GSettings", "GSetting")]
    [RequireOwner]
    public class GlobalSettingsCommand : SettingCommand
    {
        public GlobalSettingsCommand(ITypeReaderCollection readers)
            : base(readers) { }

        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.Global);
        protected override IEntity<ulong> SettingContext => SettingsManager.Global;

        [Call]
        [Usage("GLOBALSETTINGS_HELP_USAGE_DEFAULT")]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Set")]
        [Usage("GLOBALSETTINGS_HELP_USAGE_SET")]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
