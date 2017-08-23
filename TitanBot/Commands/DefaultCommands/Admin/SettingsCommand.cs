using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using TitanBot.Commands.DefaultCommands.Abstract;
using TitanBot.Replying;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Commands;
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
        Task ListSettingsAsync([Dense]string settingGroup = null, [CallFlag('g', "Group", Flags.PREFERENCES_G)]string group = null)
            => ListSettingsAsync(settingGroup, GetGroup(group));

        [Call("Toggle")]
        [Usage(Usage.SETTINGS_TOGGLE)]
        Task ToggleSettingAsync(string key, [CallFlag('g', "Group", Flags.PREFERENCES_G)]string group = null)
            => ToggleSettingAsync(key, GetGroup(group));

        [Call("Set")]
        [Usage(Usage.SETTINGS_SET)]
        Task SetSettingAsync(string key, [Dense]string value = null, [CallFlag('g', "Group", Flags.PREFERENCES_G)]string group = null)
            => SetSettingAsync(key, value, GetGroup(group));

        [Call("Group"), Alias("Groups")]
        async Task GroupsAsync([Name("Remove|Set")]string method = null, int groupId = -1, string[] names = null)
        {
            if (method == null)
                await ListGroups();
            else if (method.ToLower() == "remove")
                if (groupId < 0)
                    await ReplyAsync(SettingText.MISSING_GROUPID, ReplyType.Error);
                else
                    await RemoveGroup(groupId);
            else if (method.ToLower() == "set")
                if (groupId < 0)
                    await ReplyAsync(SettingText.MISSING_GROUPID, ReplyType.Error);
                else if (names == null || names.Length == 0)
                    await ReplyAsync(SettingText.MISSING_NAMES, ReplyType.Error);
                else
                    await SetGroup(groupId, names);
            else
                await ReplyAsync(SettingText.INVALID_METHOD, ReplyType.Error, method);
        }
    }
}
