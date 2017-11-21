using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Titanbot.Extensions.Settings.Interfaces;
using Titansmasher.Services.Display;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Models
{
    internal class SettingEditor<TSetting> : ISettingEditor<TSetting>
        where TSetting : class, new()
    {
        #region Fields

        private List<IValueEditor> _valueEditors = new List<IValueEditor>();

        #endregion Fields

        #region Constructors

        public SettingEditor(ISettingEditorManager parent)
        {
            Parent = parent;
        }

        #endregion Constructors

        #region Methods

        #region Converters

        private Func<TAccept[], TStore[]> BuildArrayConverter<TAccept, TStore>(Func<TAccept, TStore> baseConverter)
            => a => a?.Select(v => baseConverter(v))?.ToArray();

        private TStore LikeToLikeConverter<TStore>(TStore v)
            => v;

        #endregion Converters

        #region Viewers

        private IDisplayable<string> EntityViewer<TAccept>(ISettingEditorContext context, ulong id)
            => EntityViewer<TAccept>(context, (ulong?)id);

        private IDisplayable<string> EntityViewer<TAccept>(ISettingEditorContext context, ulong? id)
        {
            if (id == null)
                return new TextLiteral("");
            var tAccept = typeof(TAccept);
            var interfaces = tAccept.GetInterfaces();
            if (tAccept == typeof(IRole) || interfaces.Contains(typeof(IRole)))
                return (TextLiteral)$"<@&{id}>";
            if (tAccept == typeof(IUser) || interfaces.Contains(typeof(IUser)))
                return (TextLiteral)$"<@{id}>";
            if (tAccept == typeof(IChannel) || interfaces.Contains(typeof(IChannel)))
                return (TextLiteral)$"<#{id}>";
            if (tAccept == typeof(IGuild) || interfaces.Contains(typeof(IGuild)))
                return (TextLiteral)context.Client.GetGuild(id.Value).Name;

            return (TextLiteral)id.ToString();
        }

        #endregion Viewers

        private Action<IValueEditorBuilder<TSetting, TStore[], TAccept[]>> BuildArrayEditor<TStore, TAccept>(Action<IValueEditorBuilder<TSetting, TStore, TAccept>> templateEditor = null)
            => b =>
            {
                var temp = MakeBuilder<TStore, TAccept>(null, null);
                templateEditor?.Invoke(temp);
                b.SetName(temp.Name);
                b.SetValidator((c, a) => a.Select(v => temp.Validator?.Invoke(c, v)).FirstOrDefault(v => v != null));
                b.SetViewer((c, s) => s.Length == 0 ? null : TextLiteral.Join(", ", s.Select(v => temp.Viewer?.Invoke(c, v))));
                b.AllowGroups(temp.Groups);
            };

        private IValueEditorBuilder<TSetting, TStore, TAccept> MakeBuilder<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter)
            => new ValueEditorBuilder<TSetting, TStore, TAccept>(this, property, converter);

        private Action<IValueEditorBuilder<TSetting, TStore, TAccept>> ChainEditors<TStore, TAccept>(params Action<IValueEditorBuilder<TSetting, TStore, TAccept>>[] editors)
            => b => editors.ToList().ForEach(e => e?.Invoke(b));

        private ISettingEditor<TSetting> AddSettingInternal<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter, Action<IValueEditorBuilder<TSetting, TStore, TAccept>> builder)
        {
            return this;
        }

        #endregion Methods

        #region ISettingEditor

        public string Name { get; set; } = typeof(TSetting).Name;
        public IDisplayable<string> Description { get; set; }
        public IDisplayable<string> Notes { get; set; }
        public ISettingEditorManager Parent { get; }
        public IReadOnlyList<IValueEditor> ValueEditors => _valueEditors.AsReadOnly();

        public ISettingEditor<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter, Action<IValueEditorBuilder<TSetting, TStore, TAccept>> builder = null)
            => AddSettingInternal(property, converter, builder);

        public ISettingEditor<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Action<IValueEditorBuilder<TSetting, TStore, TStore>> builder = null)
            => AddSettingInternal(property, LikeToLikeConverter, builder);

        public ISettingEditor<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<TAccept, TStore> converter, Action<IValueEditorBuilder<TSetting, TStore, TAccept>> builder = null)
            => AddSettingInternal(property, BuildArrayConverter(converter), BuildArrayEditor(builder));

        public ISettingEditor<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore[]>> property, Action<IValueEditorBuilder<TSetting, TStore, TStore>> builder = null)
            => AddSettingInternal(property, LikeToLikeConverter, BuildArrayEditor(builder));

        public ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong>> property, Action<IValueEditorBuilder<TSetting, ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>
            => AddSettingInternal(property, a => a.Id, ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), builder));

        public ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong?>> property, Action<IValueEditorBuilder<TSetting, ulong?, TAccept>> builder = null) where TAccept : IEntity<ulong>
            => AddSettingInternal(property, a => a?.Id, ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), builder));

        public ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong[]>> property, Action<IValueEditorBuilder<TSetting, ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>
            => AddSettingInternal(property, BuildArrayConverter<TAccept, ulong>(a => a.Id), BuildArrayEditor(ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), builder)));

        public ISettingEditor<TSetting> SetDescription(IDisplayable<string> description)
        {
            Description = description;
            return this;
        }

        public ISettingEditor<TSetting> SetName(string name)
        {
            Name = name;
            return this;
        }

        public ISettingEditor<TSetting> SetNotes(IDisplayable<string> notes)
        {
            Notes = notes;
            return this;
        }

        ISettingEditor ISettingEditor.SetDescription(IDisplayable<string> description)
            => SetDescription(description);

        ISettingEditor ISettingEditor.SetName(string name)
            => SetName(name);

        ISettingEditor ISettingEditor.SetNotes(IDisplayable<string> notes)
            => SetNotes(notes);

        #endregion ISettingEditor
    }
}