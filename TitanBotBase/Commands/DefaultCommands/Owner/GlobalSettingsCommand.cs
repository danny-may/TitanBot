using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBotBase.TypeReaders;
using TitanBotBase.Util;

namespace TitanBotBase.Commands.DefaultCommands.Owner
{
    [Description("Allows the retrieval and changing of existing settings for the server")]
    [RequireOwner]
    public class GlobalSettingsCommand : Command
    {
        ITypeReaderCollection Readers { get; }
        ICommandContext Context { get; }

        public GlobalSettingsCommand(ITypeReaderCollection readers, ICommandContext context)
        {
            Readers = readers;
            Context = context;
        }

        [Call]
        [Usage("Lists all settings available")]
        async Task ListSettingsAsync([Dense]string settingGroup = null)
        {
            var builder = new EmbedBuilder
            {
                Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = Author.GetAvatarUrl(),
                    Text = $"{Author.Username} | Settings"
                }
            };

            if (settingGroup == null)
            {
                builder.WithTitle($"Please select a setting group from the following:")
                       .WithDescription(string.Join("\n", SettingsManager.EditableGlobalSettingsGroups.Select(g => g.GroupName)));
                if (string.IsNullOrWhiteSpace(builder.Description))
                    builder.Description = "No settings groups available!";
                await ReplyAsync("", embed: builder.Build());
                return;
            }
            var groups = SettingsManager.EditableGlobalSettingsGroups.Where(g => g.GroupName.ToLower() == settingGroup.ToLower());
            if (groups.Count() == 0)
            {
                await ReplyAsync($"`{settingGroup}`isnt a valid setting group!", ReplyType.Error);
                return;
            }

            builder.WithTitle($"Here are all the settings for the group `{groups.First().GroupName}`");
            foreach (var setting in groups.SelectMany(g => g.Settings))
            {
                var value = setting.Display(SettingsManager, Guild.Id);
                if (string.IsNullOrWhiteSpace(value))
                    value = "Not Set";
                builder.AddInlineField(setting.Name, value);
            }
            var descriptions = string.Join("\n", groups.Select(g => g.Description));
            var notes = string.Join("\n", groups.Select(g => g.Notes));
            if (!string.IsNullOrWhiteSpace(descriptions))
                builder.WithDescription(descriptions);
            if (!string.IsNullOrWhiteSpace(notes))
                builder.AddField("Notes", notes);
            await ReplyAsync("", embed: builder.Build());
        }

        [Call("Set")]
        [Usage("Sets the given setting to the given value.")]
        async Task SetSettingAsync(string key, [Dense]string value = null)
        {
            var setting = SettingsManager.EditableGlobalSettingsGroups.SelectMany(g => g.Settings)
                                 .FirstOrDefault(s => s.Name.ToLower() == key.ToLower());
            if (setting == null)
                await ReplyAsync($"Could not find the `{key}` setting", ReplyType.Error);
            else
            {
                var readerResult = await Readers.Read(setting.Type, Context, value);

                if (!readerResult.IsSuccess)
                {
                    await ReplyAsync($"`{value}` is not a valid value for the setting {setting.Name}", ReplyType.Error);
                    return;
                }

                var oldValue = setting.Display(SettingsManager, Guild.Id);

                if (!setting.TrySave(SettingsManager, Guild.Id, readerResult.Best, out string errors))
                    await ReplyAsync(errors, ReplyType.Error);
                else
                {
                    var newValue = setting.Display(SettingsManager, Guild.Id);
                    var builder = new EmbedBuilder
                    {
                        Title = $"{setting.Name} has changed",
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = BotUser.GetAvatarUrl(),
                            Text = BotUser.Username,
                        },
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField("Old value", string.IsNullOrWhiteSpace(oldValue) ? "Not Set" : oldValue)
                     .AddField("New value", string.IsNullOrWhiteSpace(newValue) ? "Not Set" : newValue);
                    await ReplyAsync("", embed: builder.Build());
                }
            }
        }
    }
}
