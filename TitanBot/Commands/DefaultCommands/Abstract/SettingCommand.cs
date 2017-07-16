using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using TitanBot.Util;

namespace TitanBot.Commands.DefaultCommands.Abstract
{
    public abstract class SettingCommand : Command
    {
        protected ITypeReaderCollection Readers { get; }

        protected abstract IReadOnlyList<IEditableSettingGroup> Settings { get; }
        protected abstract ulong SettingId { get; }

        public SettingCommand(ITypeReaderCollection readers)
        {
            Readers = readers;
        }

        protected async Task ListSettingsAsync(string settingGroup = null)
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
                       .WithDescription(string.Join("\n", Settings.Select(g => g.GroupName)));
                if (string.IsNullOrWhiteSpace(builder.Description))
                    builder.Description = TextResource.GetResource("SETTINGS_DESCRIPTION_NOSETTINGS");
                await ReplyAsync(Embedable.FromEmbed(builder));
                return;
            }
            var groups = Settings.Where(g => g.GroupName.ToLower() == settingGroup.ToLower());
            if (groups.Count() == 0)
            {
                await ReplyAsync("SETTINGS_INVALIDGROUP", ReplyType.Error, settingGroup);
                return;
            }

            builder.WithTitle(TextResource.Format("SETTINGS_TITLE_GROUP", groups.First().GroupName));
            foreach (var setting in groups.SelectMany(g => g.Settings))
            {
                var value = setting.Display(Context, SettingId);
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
            await ReplyAsync(Embedable.FromEmbed(builder));
        }

        protected async Task SetSettingAsync(string key, string value = null)
        {
            var setting = Settings.SelectMany(g => g.Settings)
                                 .FirstOrDefault(s => s.Name.ToLower() == key.ToLower());
            if (setting == null)
                await ReplyAsync("SETTINGS_KEY_NOTFOUND", ReplyType.Error, key);
            else
            {
                var readerResult = await Readers.Read(setting.Type, Context, value);

                if (!readerResult.IsSuccess)
                {
                    await ReplyAsync("SETTINGS_VALUE_INVALID", ReplyType.Error, setting.Name, value);
                    return;
                }

                var oldValue = setting.Display(Context, SettingId);

                if (!setting.TrySave(Context, SettingId, readerResult.Best, out string errors))
                    await ReplyAsync(errors, ReplyType.Error, value);
                else
                {
                    var newValue = setting.Display(Context, SettingId);
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
                    await ReplyAsync(Embedable.FromEmbed(builder));
                }
            }
        }
    }
}
