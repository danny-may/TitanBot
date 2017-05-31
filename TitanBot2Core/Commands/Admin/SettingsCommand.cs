using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Commands.Admin
{
    [Description("Allows the retrieval and changing of existing settings for the server")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    class SettingsCommand : Command
    {
        private readonly List<Setting> _settings;

        public SettingsCommand()
        {
            _settings = new List<Setting>()
                {
                    Setting.StringDefault("TimerText", "Titan Lord", 500, g => g.TitanLord.TimerText, (g,s) => g.TitanLord.TimerText = s),
                    Setting.StringDefault("InXText", "Titan Lord", 500, g => g.TitanLord.InXText, (g,s) => g.TitanLord.InXText = s),
                    Setting.StringDefault("NowText", "Titan Lord", 500, g => g.TitanLord.NowText, (g,s) => g.TitanLord.NowText = s),
                    Setting.StringDefault("RoundText", "Titan Lord", 500, g => g.TitanLord.RoundText, (g,s) => g.TitanLord.RoundText = s, "Use %TIME% for the time, %USER% for the user, and %ROUND% for the round"),
                    Setting.IntDefault("ClanQuest", "Titan Lord", g => g.TitanLord.CQ, (g,v) => g.TitanLord.CQ = v),
                    Setting.UlongDefault("PermissionOverride", "General", g => g.PermOverride, (g,v) => g.PermOverride = v),
                    Setting.BoolDefault("PinTimer", "Titan Lord", g => g.TitanLord.PinTimer, (g,v) => g.TitanLord.PinTimer = v, "NOTE: Wont pin if it doesnt have permission"),
                    Setting.ChannelDefault("TimerChannel", "Titan Lord", g => g.TitanLord.Channel ?? 0, (g,v) => g.TitanLord.Channel = v),
                    Setting.Default("CustomPrefix", "General", g => g.Prefix, (g,s) => $"Please use `{Context.Prefix}prefix <new prefix>` to set the prefix"),
                    new Setting
                    {
                        Key = "PrePings",
                        Group = "Titan Lord",
                        Get = g => string.Join(", ", g.TitanLord.PrePings.Select(p => new TimeSpan(0, 0, p).Beautify())),
                        Set = (g, s) =>
                        {
                            var ints = s.Split(' ', true)
                                        .Select(t => Readers.Read(typeof(TimeSpan), Context, t).Result)
                                        .Where(r => r.IsSuccess)
                                        .SelectMany(r => r.Values)
                                        .Select(t => t.Value as TimeSpan?)
                                        .Where(t => t != null)
                                        .Cast<TimeSpan>()
                                        .Select(t => (int)t.TotalSeconds);

                            if (ints.Count() == 0)
                                return "Invalid times specified.";

                            g.TitanLord.PrePings = ints.ToArray();

                            return null;
                        }
                    }
            };
        }

        [Call]
        [Usage("Lists all settings available")]
        async Task ListSettingsAsync([Dense]string settingGroup = null)
        {
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Res.Emoji.Information_source,
                    Name = "Settings"
                },
                Color = System.Drawing.Color.LightSkyBlue.ToDiscord(),
                Timestamp = DateTime.Now,
                Title = "These are the settings for this guild",
                Description = $"Use `{Context.Prefix}settings set <setting> <value>` to change the value for a setting"
            };
            foreach (var group in _settings.GroupBy(s => s.Group))
            {
                embed.AddField($"{group.Key} Settings", string.Join("\n", group.Select(s => $"**> __{s.Key}__**\n{s.Get(Context.GuildData)}" + (!string.IsNullOrWhiteSpace(s.Notes) ? $"\n*{s.Notes}*" : ""))));
            }

            await ReplyAsync("", embed: embed.Build());
        }

        [Call("Set")]
        [Usage("Sets the given setting to the given value.")]
        async Task SetSettingAsync(string key, [Dense] string value)
        {
            var setting = _settings.SingleOrDefault(s => s.Key.ToLower() == key.ToLower());

            if (setting == null)
            {
                await ReplyAsync($"The setting `{key}` could not be found. Please use `{Context.Prefix}settings list` for a list of all available settings", ReplyType.Error);
                return;
            }

            var result = setting.Set(Context.GuildData, value);

            if (result == null)
            {
                await Context.Database.Guilds.Update(Context.GuildData);
                await ReplyAsync($"Set `{setting.Key}` to {setting.Get(Context.GuildData)}", ReplyType.Success);
            }
            else
            {
                await ReplyAsync($"Unable to set `{setting.Key}` to {value}. {result}", ReplyType.Error);
            }
        }

        private class Setting
        {
            public string Group { get; set; }
            public string Key { get; set; }
            public string Notes { get; set; }
            public Func<Guild, string> Get { get; set; }
            public Func<Guild, string, string> Set { get; set; }

            public static Setting Default(string key, string group, Func<Guild, string> getter, Func<Guild, string, string> setter, string notes = "")
            => new Setting
            {
                Key = key,
                Group = group,
                Get = getter,
                Set = setter,
                Notes = notes
            };

            public static Setting StringDefault(string key, string group, int maxlength, Func<Guild, string> getter, Action<Guild, string> setter, string notes = "")
                => Default(key, group, getter, (g, s) =>
                {
                    if (s.Length < maxlength && maxlength > 0)
                        setter(g, s);
                    else
                        return $"The text supplied is too long. Limit set to {maxlength} characters (including formatting characters)";
                    return null;
                }, notes);

            public static Setting IntDefault(string key, string group, Func<Guild, int> getter, Action<Guild, int> setter, string notes = "")
                => Default(key, group, g => getter(g).ToString(), (g, s) =>
                {
                    int val;
                    if (int.TryParse(s, out val))
                        setter(g, val);
                    else
                        return "Please supply a valid number.";
                    return null;
                }, notes);

            public static Setting UlongDefault(string key, string group, Func<Guild, ulong> getter, Action<Guild, ulong> setter, string notes = "")
                => Default(key, group, g => getter(g).ToString(), (g, s) =>
                {
                    ulong val;
                    if (ulong.TryParse(s, out val))
                        setter(g, val);
                    else
                        return "Please supply a valid number.";
                    return null;
                }, notes);

            public static Setting BoolDefault(string key, string group, Func<Guild, bool> getter, Action<Guild, bool> setter, string notes = "")
                => Default(key, group, g => getter(g).ToString(), (g, s) =>
                {
                    bool val;
                    if (bool.TryParse(s, out val))
                        setter(g, val);
                    else
                        return "Please supply a valid boolean (true or false)";
                    return null;
                }, notes);

            public static Setting ChannelDefault(string key, string group, Func<Guild, ulong> getter, Action<Guild, ulong> setter, string notes = "")
                => Default(key, group, g =>
                {
                    var val = getter(g);
                    if (val == 0)
                        return "Not set";
                    return MentionUtils.MentionChannel(val);
                }, (g, s) =>
                {
                    ulong val;
                    if (MentionUtils.TryParseChannel(s, out val))
                        setter(g, val);
                    else
                        return "That channel does not exist.";
                    return null;
                }, notes);

            public static Setting UserDefault(string key, string group, Func<Guild, ulong> getter, Action<Guild, ulong> setter, string notes = "")
                => Default(key, group, g =>
                {
                    var val = getter(g);
                    if (val == 0)
                        return "Not set";
                    return MentionUtils.MentionUser(val);
                }, (g, s) =>
                {
                    ulong val;
                    if (MentionUtils.TryParseUser(s, out val))
                        setter(g, val);
                    else
                        return "Invalid user";
                    return null;
                }, notes);

            public static Setting RoleDefault(string key, string group, Func<Guild, ulong> getter, Action<Guild, ulong> setter, string notes = "")
                => Default(key, group, g =>
                {
                    var val = getter(g);
                    if (val == 0)
                        return "Not set";
                    return MentionUtils.MentionRole(val);
                }, (g, s) =>
                {
                    ulong val;
                    if (MentionUtils.TryParseRole(s, out val))
                        setter(g, val);
                    else
                        return "Invalid role";
                    return null;
                }, notes);
        }
    }
}
