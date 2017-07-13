using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.TypeReaders;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Admin
{
    [Description("SETTINGS_HELP_DESCRIPTION")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    public class SettingsCommand : Command
    {
        ITypeReaderCollection Readers { get; }
        ICommandContext Context { get; }

        public SettingsCommand(ITypeReaderCollection readers, ICommandContext context)
        {
            Readers = readers;
            Context = context;
        }

        [Call]
        [Usage("SETTINGS_HELP_USAGE_DEFAULT")]
        async Task ListSettingsAsync([Dense]string settingGroup = null)
        {
            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = BotUser.GetAvatarUrl(),
                    Text = TextResource.Format("EMBED_FOOTER", BotUser.Username, "Settings")
                }
            };

            if (settingGroup == null)
            {
                builder.WithTitle(TextResource.GetResource("SETTINGS_TITLE_NOGROUP"))
                       .WithDescription(string.Join("\n", SettingsManager.EditableSettingGroups.Select(g => g.GroupName)));
                if (string.IsNullOrWhiteSpace(builder.Description))
                    builder.Description = TextResource.GetResource("SETTINGS_DESCRIPTION_NOSETTINGS");
                await ReplyAsync("", embed: builder.Build());
                return;
            }
            var groups = SettingsManager.EditableSettingGroups.Where(g => g.GroupName.ToLower() == settingGroup.ToLower());
            if (groups.Count() == 0)
            {
                await ReplyAsync(TextResource.Format("SETTINGS_INVALIDGROUP", ReplyType.Error, settingGroup));
                return;
            }

            builder.WithTitle(TextResource.Format("SETTINGS_TITLE_GROUP", groups.First().GroupName));
            foreach (var setting in groups.SelectMany(g => g.Settings))
            {
                var value = setting.Display(SettingsManager, Guild.Id);
                if (string.IsNullOrWhiteSpace(value))
                    value = TextResource.GetResource("SETTINGS_NOTSET");
                builder.AddInlineField(setting.Name, value);
            }
            var descriptions = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Description))
                                                       .Select(g => TextResource.GetResource(g.Description)));
            var notes = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Notes))
                                                .Select(g => TextResource.GetResource(g.Notes)));
            if (!string.IsNullOrWhiteSpace(descriptions))
                builder.WithDescription(descriptions);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField(TextResource.GetResource("NOTES"), notes);
            await ReplyAsync("", embed: builder.Build());
        }

        [Call("Set")]
        [Usage("SETTINGS_HELP_USAGE_SET")]
        async Task SetSettingAsync(string key, [Dense]string value = null)
        {
            var setting = SettingsManager.EditableSettingGroups.SelectMany(g => g.Settings)
                                 .FirstOrDefault(s => s.Name.ToLower() == key.ToLower());
            if (setting == null)
                await ReplyAsync(TextResource.Format("SETTINGS_KEY_NOTFOUND", ReplyType.Error, key));
            else
            {
                var readerResult = await Readers.Read(setting.Type, Context, value);

                if (!readerResult.IsSuccess)
                {
                    await ReplyAsync(TextResource.Format("SETTINGS_VALUE_INVALID", ReplyType.Error, setting.Name, value));
                    return;
                }

                var oldValue = setting.Display(SettingsManager, Guild.Id);

                if (!setting.TrySave(SettingsManager, Guild.Id, readerResult.Best, out string errors))
                    await ReplyAsync(errors, ReplyType.Error); //TODO: Fix this bit to use the TextResource
                else
                {
                    var newValue = setting.Display(SettingsManager, Guild.Id);
                    var builder = new EmbedBuilder
                    {
                        Title = TextResource.Format("SETTINGS_VALUE_CHANGED_TITLE", setting.Name),
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = BotUser.GetAvatarUrl(),
                            Text = BotUser.Username,
                        },
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField(TextResource.GetResource("SETTING_VALUE_OLD"), string.IsNullOrWhiteSpace(oldValue) ? TextResource.GetResource("SETTINGS_NOTSET") : oldValue)
                     .AddField(TextResource.GetResource("SETTING_VALUE_NEW"), string.IsNullOrWhiteSpace(newValue) ? TextResource.GetResource("SETTINGS_NOTSET") : newValue);
                    await ReplyAsync("", embed: builder.Build());
                }
            }
        }
    }
}
