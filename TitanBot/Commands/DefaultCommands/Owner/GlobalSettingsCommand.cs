using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.TypeReaders;
using TitanBot.Util;
using System.Collections.Generic;
using TitanBot.Settings;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description("GLOBALSETTINGS_HELP_DESCRIPTION")]
    [Alias("GSettings", "GSetting")]
    [RequireOwner]
    public class GlobalSettingsCommand : SettingCommand
    {
        protected override IReadOnlyList<IEditableSettingGroup> Settings => SettingsManager.GlobalSettingGroups;
        protected override ulong SettingId => 1;

        public GlobalSettingsCommand(ITypeReaderCollection readers)
            : base(readers) { }

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
