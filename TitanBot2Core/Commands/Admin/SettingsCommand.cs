using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database.Tables;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Admin
{
    [Description("Allows the retrieval and changing of existing settings for the server")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    class SettingsCommand : Command
    {
        private static List<SettingGroup> _settingGroups { get; }

        static SettingsCommand()
        {
            _settingGroups = new List<SettingGroup>
            {
                new SettingGroup("TitanLord", "There are several format strings you can use to have live data in your message.\n" +
                                              "Use `%USER%` to include the user who started the timer\n" +
                                              "Use `%TIME%` to include how long until the titan lord is up\n" +
                                              "Use `%ROUND%` for the round number\n" +
                                              "Use `%CQ%` for the current CQ number\n" +
                                              "Use `%COMPLETE%` for the time the titan lord will be up (UTC time)")
                         .Create(g => g.TitanLord.TimerText, v => v.Length > 500 ? "You cannot have more than 500 characters for this setting" : null)
                         .Create(g => g.TitanLord.InXText, v => v.Length > 500 ? "You cannot have more than 500 characters for this setting" : null)
                         .Create(g => g.TitanLord.NowText, v => v.Length > 500 ? "You cannot have more than 500 characters for this setting" : null)
                         .Create(g => g.TitanLord.RoundText, v => v.Length > 500 ? "You cannot have more than 500 characters for this setting" : null)
                         .Create(g => g.TitanLord.PinTimer)
                         .Create(g => g.TitanLord.RoundPings)
                         .Create(g => g.TitanLord.PrePings, (TimeSpan[] t) => t.Select(v => (int)v.TotalSeconds).ToArray(), (v,c) => string.Join(", ", v.Select(t => new TimeSpan(0,0,t))))
                         .Create("ClanQuest", g => g.TitanLord.CQ, v => v < 0 ? "Your clan quest cannot be negative" : null)
                         .Create("TimerChannel", g => g.TitanLord.Channel, (IMessageChannel c) => c?.Id, (v, c) => v == null ? "" : $"<#{v}>"),
                new SettingGroup("General").Create(g => g.PermOverride)
                                           .Create(g => g.Prefix)
                                           .Create(g => g.NotifyAlive, (IMessageChannel c) => c?.Id, (v, c) => v == null ? "" : $"<#{v}>")
                                           .Create(g => g.NotifyDead, (IMessageChannel c) => c?.Id, (v, c) => v == null ? "" : $"<#{v}>")
            };
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
                    IconUrl = Context.User.GetAvatarUrl(),
                    Text = $"{Context.User.Username} | Settings"
                }
            };

            if (settingGroup == null)
            {
                builder.WithTitle($"Please select a setting group from the following:")
                       .WithDescription(string.Join("\n", _settingGroups.Select(g => g.GroupName)));
                await ReplyAsync("", embed: builder.Build());
                return;
            }
            var group = _settingGroups.FirstOrDefault(g => g.GroupName.ToLower() == settingGroup.ToLower());
            if (group == null)
            {
                await ReplyAsync("That isnt a valid setting group!");
                return;
            }

            builder.WithTitle($"Here are all the settings for the group `{group.GroupName}`");
            foreach (var setting in group.Settings)
            {
                var value = setting.Load(Context);
                if (string.IsNullOrWhiteSpace(value))
                    value = "Not Set";
                builder.AddInlineField(setting.Name, value);
            }
            if (!string.IsNullOrWhiteSpace(group.Description))
                builder.AddField("Notes", group.Description);
            await ReplyAsync("", embed: builder.Build());
        }

        [Call("Set")]
        [Usage("Sets the given setting to the given value.")]
        async Task SetSettingAsync(string key, [Dense]string value = null)
        {
            var setting = _settingGroups.SelectMany(g => g.Settings)
                                        .FirstOrDefault(s => s.Name.ToLower() == key.ToLower());
            if (setting == null)
                await ReplyAsync("Could not find setting", ReplyType.Error);
            else
            {
                var oldValue = setting.Load(Context);

                if (!setting.Save(Context, value, out string errors))
                    await ReplyAsync(errors, ReplyType.Error);
                else
                {
                    var builder = new EmbedBuilder
                    {
                        Title = $"{setting.Name} has changed",
                        Footer = new EmbedFooterBuilder
                        {
                            IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                            Text = Context.Client.CurrentUser.Username,
                        },
                        Timestamp = DateTime.Now,
                        Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                    }.AddField("Old value", oldValue)
                     .AddField("New value", setting.Load(Context));
                    await ReplyAsync("", embed: builder.Build());
                }
            }
        }

        private abstract class Setting
        {
            public string Name { get; protected set; }

            public abstract bool Save(CmdContext context, string text, out string errors);
            public abstract string Load(CmdContext context);

            public static SettingGroup BuildGroup(string groupName, string description)
                => new SettingGroup(groupName, description);

            public static Setting Create<Ts, Ta>(string name, Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ts, CmdContext, string> viewer, Func<Ta, string> validator)
                => new TypedSetting<Ts, Ta>(name, property, converter, viewer, validator);

            //TStore is the type that the value is stored in the database as
            //TAccept is the data type that is permitted by the setting
            //Func<TAccept, TStore> Converter is used to go from the accepted data type to the stored data type
            //TStore is calculated by the type of the property given to property
            //TAccept is calculated by the input type of the converter
            private class TypedSetting<TStore, TAccept> : Setting
            {
                private Func<TAccept, string> Validator { get; }
                private Func<TAccept, TStore> Converter { get; }
                private Func<TStore, CmdContext, string> Viewer { get; }
                private Action<Guild, TStore> Setter { get; }
                private Func<Guild, TStore> Getter { get; }

                public TypedSetting(string name, Expression<Func<Guild, TStore>> property, Func<TAccept, TStore> converter, Func<TStore, CmdContext, string> viewer, Func<TAccept, string> validator)
                {
                    Name = name;
                    Setter = CreateSetter(property);
                    Getter = property.Compile();
                    Viewer = viewer;
                    Validator = validator;
                    Converter = converter;
                }

                public override bool Save(CmdContext context, string text, out string errors)
                {
                    var acceptType = typeof(TAccept);
                    TAccept result = default(TAccept);
                    if (text == null && (!acceptType.IsValueType || (Nullable.GetUnderlyingType(acceptType) != null)))
                        errors = null;
                    else
                    {
                        var readRes = context.Readers.Read(acceptType, context, text).Result;
                        if (!readRes.IsSuccess)
                            errors = $"`{text}` is not a valid value for {Name}";
                        else
                        {
                            errors = Validator((TAccept)readRes.Best);
                            result = (TAccept)readRes.Best;
                        }
                    }

                    if (errors != null)
                        return false;

                    Setter(context.GuildData, Converter(result));
                    context.Database.Guilds.Upsert(context.GuildData).Wait();
                    return true;
                }

                public override string Load(CmdContext context)
                    => Viewer(Getter(context.GuildData), context);

                private Action<Guild, TStore> CreateSetter(Expression<Func<Guild, TStore>> selector)
                {
                    var valueParam = Expression.Parameter(typeof(TStore));
                    var body = Expression.Assign(selector.Body, valueParam);
                    return Expression.Lambda<Action<Guild, TStore>>(body,
                        selector.Parameters.Single(),
                        valueParam).Compile();
                }
            }
        }

        private class SettingGroup
        {
            public string GroupName { get; }
            public string Description { get; }
            public List<Setting> Settings { get; } = new List<Setting>();

            internal SettingGroup(string groupName, string description)
            {
                GroupName = groupName;
                Description = description;
            }
            internal SettingGroup(string groupName)
            {
                GroupName = groupName;
            }

            private string GetName<T>(Expression<Func<Guild, T>> property)
                => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";

            public SettingGroup Create<T>(Expression<Func<Guild, T>> property)
                => Create(GetName(property), property);
            public SettingGroup Create<T>(string name, Expression<Func<Guild, T>> property)
                => Create(name, property, (v, c) => v?.ToString(), t => null);
            public SettingGroup Create<T>(Expression<Func<Guild, T>> property, Func<T, CmdContext, string> viewer)
                => Create(GetName(property), property, viewer);
            public SettingGroup Create<T>(string name, Expression<Func<Guild, T>> property, Func<T, CmdContext, string> viewer)
                => Create(name, property, viewer, t => null);
            public SettingGroup Create<T>(Expression<Func<Guild, T>> property, Func<T, string> validator)
                => Create(GetName(property), property, validator);
            public SettingGroup Create<T>(string name, Expression<Func<Guild, T>> property, Func<T, string> validator)
                => Create(name, property, (v, c) => v?.ToString(), validator);
            public SettingGroup Create<T>(Expression<Func<Guild, T>> property, Func<T, CmdContext, string> viewer, Func<T, string> validator)
                => Create(GetName(property), property, viewer, validator);
            public SettingGroup Create<T>(string name, Expression<Func<Guild, T>> property, Func<T, CmdContext, string> viewer, Func<T, string> validator)
                => Create(name, property, t => t, viewer, validator);

            public SettingGroup Create<Ta, Ts>(Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter)
                => Create(GetName(property), property, converter);
            public SettingGroup Create<Ta, Ts>(string name, Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter)
                => Create(name, property, converter, (v, c) => v?.ToString(), t => null);
            public SettingGroup Create<Ta, Ts>(Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ts, CmdContext, string> viewer)
                => Create(GetName(property), property, converter, viewer);
            public SettingGroup Create<Ta, Ts>(string name, Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ts, CmdContext, string> viewer)
                => Create(name, property, converter, viewer, t => null);
            public SettingGroup Create<Ta, Ts>(Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ta, string> validator)
                => Create(GetName(property), property, converter, validator);
            public SettingGroup Create<Ta, Ts>(string name, Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ta, string> validator)
                => Create(name, property, converter, (v, c) => v?.ToString(), validator);
            public SettingGroup Create<Ta, Ts>(Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ts, CmdContext, string> viewer, Func<Ta, string> validator)
                => Create(GetName(property), property, converter, viewer, validator);

            public SettingGroup Create<Ts, Ta>(string name, Expression<Func<Guild, Ts>> property, Func<Ta, Ts> converter, Func<Ts, CmdContext, string> viewer, Func<Ta, string> validator)
            {
                Settings.Add(Setting.Create(name, property, converter, viewer, validator));
                return this;
            }
        }
    }
}
