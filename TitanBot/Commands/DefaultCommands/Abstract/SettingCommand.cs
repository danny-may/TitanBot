using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Settings;
using TitanBot.TypeReaders;

namespace TitanBot.Commands.DefaultCommands.Abstract
{
    public abstract class SettingCommand : Command
    {
        protected ITypeReaderCollection Readers { get; }

        protected abstract IReadOnlyList<ISettingEditorCollection> Settings { get; }
        protected abstract IEntity<ulong> SettingContext { get; }

        public SettingCommand(ITypeReaderCollection readers)
        {
            Readers = readers;
        }

        protected ISettingEditor Find(string key)
        {
            var editors = Settings.SelectMany(s => s);
            var editor = editors.FirstOrDefault(e => e.Name.ToLower() == key.ToLower());
            if (editor != null)
                return editor;
            editor = editors.FirstOrDefault(e => e.Aliases?.ToLower().Contains(key.ToLower()) ?? false);
            return editor;
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
                    Text = TextResource.Format(TitanBotResource.EMBED_FOOTER, BotUser.Username, "Settings")
                }
            };

            if (Settings.Count == 1)
                settingGroup = Settings.First().Name;

            if (settingGroup == null)
            {
                builder.WithTitle(TextResource.GetResource(TitanBotResource.SETTINGS_TITLE_NOGROUP))
                       .WithDescription(string.Join("\n", Settings.Select(g => g.Name)));
                if (string.IsNullOrWhiteSpace(builder.Description))
                    builder.Description = TextResource.GetResource(TitanBotResource.SETTINGS_DESCRIPTION_NOSETTINGS);
                await ReplyAsync(builder);
                return;
            }
            var groups = Settings.Where(g => g.Name.ToLower() == settingGroup.ToLower());
            if (groups.Count() == 0)
            {
                await ReplyAsync(TitanBotResource.SETTINGS_INVALIDGROUP, ReplyType.Error, settingGroup);
                return;
            }
            
            builder.WithTitle(TextResource.Format(TitanBotResource.SETTINGS_TITLE_GROUP, groups.First().Name));
            foreach (var setting in groups.SelectMany(s => s))
            {
                var value = setting.Display(Context, SettingContext);
                if (string.IsNullOrWhiteSpace(value))
                    value = TextResource.GetResource(TitanBotResource.SETTINGS_NOTSET);
                builder.AddInlineField(setting.Name, value);
            }
            var descriptions = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Description))
                                                       .Select(g => TextResource.GetResource(g.Description)));
            var notes = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Notes))
                                                .Select(g => TextResource.GetResource(g.Notes)));
            if (!string.IsNullOrWhiteSpace(descriptions))
                builder.WithDescription(descriptions);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField(TextResource.GetResource(TitanBotResource.NOTES), notes);
            await ReplyAsync(builder);
        }

        protected async Task ToggleSettingAsync(string key)
        {
            var setting = Find(key);
            if (setting == null)
                await ReplyAsync(TitanBotResource.SETTINGS_KEY_NOTFOUND, ReplyType.Error, key);
            else if (setting.Type != typeof(bool))
                await ReplyAsync(TitanBotResource.SETTINGS_UNABLE_TOGGLE, ReplyType.Error, key);
            else
            {
                var oldValue = (bool)setting.Get(SettingContext);
                var oldDisplay = setting.Display(Context, SettingContext);
                if (!setting.TrySet(Context, SettingContext, !oldValue, out string errors))
                    await ReplyAsync(errors, ReplyType.Error, !oldValue);
                else
                {
                    var newDisplay = setting.Display(Context, SettingContext);
                    var builder = new EmbedBuilder
                    {
                        Title = TextResource.Format(TitanBotResource.SETTINGS_VALUE_CHANGED_TITLE, setting.Name),
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = BotUser.GetAvatarUrl(),
                            Text = BotUser.Username,
                        },
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField(TextResource.GetResource(TitanBotResource.SETTING_VALUE_OLD), string.IsNullOrWhiteSpace(oldDisplay) ? TextResource.GetResource(TitanBotResource.SETTINGS_NOTSET) : oldDisplay)
                     .AddField(TextResource.GetResource(TitanBotResource.SETTING_VALUE_NEW), string.IsNullOrWhiteSpace(newDisplay) ? TextResource.GetResource(TitanBotResource.SETTINGS_NOTSET) : newDisplay);
                    await ReplyAsync(builder);
                }
            }
        }

        protected async Task SetSettingAsync(string key, string value = null)
        {
            var setting = Find(key);
            if (setting == null)
                await ReplyAsync(TitanBotResource.SETTINGS_KEY_NOTFOUND, ReplyType.Error, key);
            else
            {
                var readerResult = await Readers.Read(setting.Type, Context, value);
            
                if (!readerResult.IsSuccess)
                {
                    await ReplyAsync(TitanBotResource.SETTINGS_VALUE_INVALID, ReplyType.Error, setting.Name, value);
                    return;
                }
            
                var oldValue = setting.Display(Context, SettingContext);
            
                if (!setting.TrySet(Context, SettingContext, readerResult.Best, out string errors))
                    await ReplyAsync(errors, ReplyType.Error, value);
                else
                {
                    var newValue = setting.Display(Context, SettingContext);
                    var builder = new EmbedBuilder
                    {
                        Title = TextResource.Format(TitanBotResource.SETTINGS_VALUE_CHANGED_TITLE, setting.Name),
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = BotUser.GetAvatarUrl(),
                            Text = BotUser.Username,
                        },
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField(TextResource.GetResource(TitanBotResource.SETTING_VALUE_OLD), string.IsNullOrWhiteSpace(oldValue) ? TextResource.GetResource(TitanBotResource.SETTINGS_NOTSET) : oldValue)
                     .AddField(TextResource.GetResource(TitanBotResource.SETTING_VALUE_NEW), string.IsNullOrWhiteSpace(newValue) ? TextResource.GetResource(TitanBotResource.SETTINGS_NOTSET) : newValue);
                    await ReplyAsync(builder);
                }
            }
        }
    }
}
