using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Settings;
using TitanBot.TypeReaders;
using static TitanBot.TBLocalisation.Commands;

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
            var builder = new LocalisedEmbedBuilder
            {
                Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                Timestamp = DateTime.Now,
                Footer = new LocalisedFooterBuilder().WithRawIconUrl(BotUser.GetAvatarUrl())
                                                     .WithText((SettingText.FOOTERTEXT, BotUser.Username))
            };

            if (Settings.Count == 1)
                settingGroup = Settings.First().Name;

            if (settingGroup == null)
            {
                builder.WithTitle(SettingText.TITLE_NOGROUP);
                var desc = string.Join("\n", Settings.Select(g => g.Name));
                if (string.IsNullOrWhiteSpace(desc))
                    builder.WithDescription(SettingText.DESCRIPTION_NOSETTINGS);
                else
                    builder.WithRawDescription(desc);
                await ReplyAsync(builder);
                return;
            }
            var groups = Settings.Where(g => g.Name.ToLower() == settingGroup.ToLower());
            if (groups.Count() == 0)
            {
                await ReplyAsync(SettingText.INVALIDGROUP, ReplyType.Error, settingGroup);
                return;
            }
            
            builder.WithTitle(SettingText.TITLE_GROUP, groups.First().Name);
            foreach (var setting in groups.SelectMany(s => s))
            {
                string value = setting.Display(Context, SettingContext);
                if (string.IsNullOrWhiteSpace(value))
                    builder.AddInlineField(f => f.WithRawName(setting.Name).WithValue(SettingText.NOTSET));
                else
                    builder.AddInlineField(f => f.WithRawName(setting.Name).WithRawValue(value));
            }
            var descriptions = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Description))
                                                       .Select(g => TextResource.GetResource(g.Description)));
            var notes = string.Join("\n", groups.Where(g => !string.IsNullOrWhiteSpace(g.Notes))
                                                .Select(g => TextResource.GetResource(g.Notes)));
            if (!string.IsNullOrWhiteSpace(descriptions))
                builder.WithRawDescription(descriptions);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField(f => f.WithName(TBLocalisation.NOTES).WithRawValue(notes));
            await ReplyAsync(builder);
        }

        protected async Task ToggleSettingAsync(string key)
        {
            var setting = Find(key);
            if (setting == null)
                await ReplyAsync(SettingText.KEY_NOTFOUND, ReplyType.Error, key);
            else if (setting.Type != typeof(bool))
                await ReplyAsync(SettingText.UNABLE_TOGGLE, ReplyType.Error, key);
            else
            {
                var oldValue = (bool)setting.Get(SettingContext);
                var oldDisplay = setting.Display(Context, SettingContext);
                if (!setting.TrySet(Context, SettingContext, !oldValue, out string errors))
                    await ReplyAsync(errors, ReplyType.Error, !oldValue);
                else
                {
                    var newDisplay = setting.Display(Context, SettingContext);
                    var builder = new LocalisedEmbedBuilder
                    {
                        Title = (SettingText.VALUE_CHANGED_TITLE, setting.Name),
                        Footer = new LocalisedFooterBuilder().WithIconUrl(BotUser.GetAvatarUrl()).WithText(BotUser.Username),
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField(f => f.WithName(SettingText.VALUE_OLD).WithValue(string.IsNullOrWhiteSpace(oldDisplay) ? (LocalisedString)SettingText.NOTSET : (RawString)oldDisplay))
                     .AddField(f => f.WithName(SettingText.VALUE_NEW).WithValue(string.IsNullOrWhiteSpace(newDisplay) ? (LocalisedString)SettingText.NOTSET : (RawString)newDisplay));
                    await ReplyAsync(builder);
                }
            }
        }

        protected async Task SetSettingAsync(string key, string value = null)
        {
            var setting = Find(key);
            if (setting == null)
                await ReplyAsync(SettingText.KEY_NOTFOUND, ReplyType.Error, key);
            else
            {
                var readerResult = await Readers.Read(setting.Type, Context, value);
            
                if (!readerResult.IsSuccess)
                {
                    await ReplyAsync(SettingText.VALUE_INVALID, ReplyType.Error, setting.Name, value);
                    return;
                }
            
                var oldValue = setting.Display(Context, SettingContext);
            
                if (!setting.TrySet(Context, SettingContext, readerResult.Best, out string errors))
                    await ReplyAsync(errors, ReplyType.Error, value);
                else
                {
                    var newValue = setting.Display(Context, SettingContext);
                    var builder = new LocalisedEmbedBuilder
                    {
                        Title = (SettingText.VALUE_CHANGED_TITLE, setting.Name),
                        Footer = new LocalisedFooterBuilder().WithIconUrl(BotUser.GetAvatarUrl()).WithText(BotUser.Username),
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField(f => f.WithName(SettingText.VALUE_OLD).WithValue(string.IsNullOrWhiteSpace(oldValue) ? (LocalisedString)SettingText.NOTSET : (RawString)oldValue))
                     .AddField(f => f.WithName(SettingText.VALUE_NEW).WithValue(string.IsNullOrWhiteSpace(newValue) ? (LocalisedString)SettingText.NOTSET : (RawString)newValue));
                    await ReplyAsync(builder);
                }
            }
        }
    }
}
