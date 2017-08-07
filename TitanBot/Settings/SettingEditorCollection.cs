using Discord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Contexts;
using TitanBot.Util;

namespace TitanBot.Settings
{
    class SettingEditorCollection<TSetting> : ISettingEditorCollection<TSetting>
    {
        public ISettingEditor this[int index] => SettingEditors[index];
        public int Count => SettingEditors.Count;

        public string Name { get; private set; } = typeof(TSetting).Name;
        public string Description { get; private set; }
        public string Notes { get; private set; }
        public ISettingManager Parent { get; }

        private List<ISettingEditor> SettingEditors { get; } = new List<ISettingEditor>();

        public SettingEditorCollection(ISettingManager parent)
        {
            Parent = parent;
        }

        private ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(ISettingEditorBuilder<TStore, TAccept> builder, Action<ISettingEditorBuilder<TStore, TAccept>> builderEdits = null)
        {
            builderEdits?.Invoke(builder);
            SettingEditors.Add(builder.Build());
            return this;
        }

        //Delegates
        private Func<IMessageContext, TAccept[], TStore[]> MakeArrayConverter<TStore, TAccept>(Func<IMessageContext, TAccept, TStore> converter)
            => (c, a) => a.Select(v => converter(c, v)).ToArray();

        private Func<IMessageContext, TAccept, TStore> WrapConverter<TStore, TAccept>(Func<TAccept, TStore> converter)
            => (c, a) => converter(a);

        private Action<ISettingEditorBuilder<TStore[], TAccept[]>> MakeArrayEditor<TStore, TAccept>(Action<ISettingEditorBuilder<TStore, TAccept>> templateEditor = null)
            => b =>
            {
                var temp = MakeBuilder<TStore, TAccept>(null, null);
                templateEditor?.Invoke(temp);
                b.SetName(temp.Name);
                b.SetValidator((c, a) => a.Select(v => temp.Validator(c, v)).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)));
                b.SetViewer((c, s) => string.Join(", ", s.Select(v => temp.Viewer(c, v))));
            };

        private string EntityViewer<TAccept>(IMessageContext context, ulong id)
            => EntityViewer<TAccept>(context, (ulong?)id);
        private string EntityViewer<TAccept>(IMessageContext context, ulong? id)
        {
            if (id == null)
                return null;
            var tAccept = typeof(TAccept);
            var interfaces = tAccept.GetInterfaces();
            if (tAccept == typeof(IRole) || interfaces.Contains(typeof(IRole)))
                return id == null ? null : $"<@&{id}>";
            if (tAccept == typeof(IUser) || interfaces.Contains(typeof(IUser)))
                return id == null ? null : $"<@{id}>";
            if (tAccept == typeof(IChannel) || interfaces.Contains(typeof(IChannel)))
                return id == null ? null : $"<#{id}>";
            if (tAccept == typeof(IGuild) || interfaces.Contains(typeof(IGuild)))
                return id == null ? null : context.Client.GetGuild(id.Value)?.Name;

            return id.ToString();
        }

        private ISettingEditorBuilder<TStore, TAccept> MakeBuilder<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<IMessageContext, TAccept, TStore> converter)
            => new SettingEditorBuilder<TSetting, TStore, TAccept>(Parent, property, converter);

        private Action<ISettingEditorBuilder<TStore, TAccept>> ChainEditors<TStore, TAccept>(params Action<ISettingEditorBuilder<TStore, TAccept>>[] editors)
            => b => editors.ForEach(e => e?.Invoke(b));


        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<IMessageContext, TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> editor = null)
            => AddSetting(MakeBuilder(property, converter), editor);
        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> editor = null)
            => AddSetting(property, WrapConverter(converter), editor);
        public ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Action<ISettingEditorBuilder<TStore, TStore>> editor = null)
            => AddSetting(property, a => a, editor);
        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<IMessageContext, TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> editor = null)
            => AddSetting(property, MakeArrayConverter(converter), MakeArrayEditor(editor));
        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> editor = null)
            => AddSetting(property, WrapConverter(converter), editor);
        public ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore[]>> property, Action<ISettingEditorBuilder<TStore, TStore>> editor = null)
            => AddSetting(property, a => a, editor);

        public ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong>> property, Action<ISettingEditorBuilder<ulong, TAccept>> editor = null) where TAccept : IEntity<ulong>
            => AddSetting(property, (c, a) => a.Id, ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), editor));
        public ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong?>> property, Action<ISettingEditorBuilder<ulong?, TAccept>> editor = null) where TAccept : IEntity<ulong>
            => AddSetting(property, (c, a) => a?.Id, ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), editor));
        public ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong[]>> property, Action<ISettingEditorBuilder<ulong, TAccept>> editor = null) where TAccept : IEntity<ulong>
            => AddSetting(property, (c, a) => a.Id, ChainEditors(b => b.SetViewer(EntityViewer<TAccept>), editor));

        public ISettingEditorCollection<TSetting> WithName(string name)
        {
            Name = name;
            return this;
        }

        public ISettingEditorCollection<TSetting> WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public ISettingEditorCollection<TSetting> WithNotes(string notes)
        {
            Notes = notes;
            return this;
        }

        ISettingEditorCollection ISettingEditorCollection.WithName(string name)
            => WithName(name);
        ISettingEditorCollection ISettingEditorCollection.WithDescription(string description)
            => WithDescription(description);
        ISettingEditorCollection ISettingEditorCollection.WithNotes(string notes)
            => WithNotes(notes);

        public IEnumerator<ISettingEditor> GetEnumerator()
            => SettingEditors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => SettingEditors.GetEnumerator();
    }
}
