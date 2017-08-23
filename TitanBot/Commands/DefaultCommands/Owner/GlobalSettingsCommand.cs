using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Help;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(Desc.GLOBALSETTINGS)]
    [Alias("GSettings", "GSetting")]
    [RequireOwner]
    public class GlobalSettingsCommand : SettingCommand
    {
        protected override IReadOnlyList<ISettingEditorCollection> Settings => SettingsManager.GetEditors(SettingScope.Global);
        protected override IEntity<ulong> SettingContext => SettingsManager.Global;

        public GlobalSettingsCommand(ITypeReaderCollection readers)
            : base(readers) { }

        [Call]
        [Usage(Usage.GLOBALSETTINGS_DEFAULT)]
        Task ListSettingsAsync([Dense]string settingGroup = null)
            => ListSettingsAsync(settingGroup, 0);

        [Call("Toggle")]
        [Usage(Usage.GLOBALSETTINGS_TOGGLE)]
        Task ToggleSettingAsync(string key)
            => ToggleSettingAsync(key, 0);

        [Call("Set")]
        [Usage(Usage.GLOBALSETTINGS_SET)]
        Task SetSettingAsync(string key, [Dense]string value = null)
            => SetSettingAsync(key, value, 0);
    }
}
