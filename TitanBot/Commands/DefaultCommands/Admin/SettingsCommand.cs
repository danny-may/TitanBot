using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.TypeReaders;
using TitanBot.Util;
using System.Collections.Generic;
using TitanBot.Settings;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("SETTINGS_HELP_DESCRIPTION")]
    [DefaultPermission(8)]
    [Alias("Setting")]
    [RequireContext(ContextType.Guild)]
    public class SettingsCommand : SettingCommand
    {
        protected override IReadOnlyList<IEditableSettingGroup> Settings => SettingsManager.EditableSettingGroups;

        public SettingsCommand(ITypeReaderCollection readers, ICommandContext context)
            : base(readers, context) { }

        [Call]
        [Usage("SETTINGS_HELP_USAGE_DEFAULT")]
        new Task ListSettingsAsync([Dense]string settingGroup = null)
            => base.ListSettingsAsync(settingGroup);

        [Call("Set")]
        [Usage("SETTINGS_HELP_USAGE_SET")]
        new Task SetSettingAsync(string key, [Dense]string value = null)
            => base.SetSettingAsync(key, value);
    }
}
