using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TitanBot.Commands;
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

        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(string name, Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null)
        {
            SettingEditors.Add(new SettingEditor<TSetting, TStore, TAccept>(e => Parent.GetContext(e).Get<TSetting>(),
                                                                            (e, a) => Parent.GetContext(e).Edit(a),
                                                                            name,
                                                                            property,
                                                                            converter,
                                                                            viewer,
                                                                            validator));
            return this;
        }
        public ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null)
            => AddSetting(GetName(property), property, converter, viewer, validator);
        public ISettingEditorCollection<TSetting> AddSetting<TStore>(string name, Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null)
            => AddSetting(name, property, (c, p) => p, viewer, validator);
        public ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null)
            => AddSetting(GetName(property), property, (c, p) => p, viewer, validator);

        public IEnumerator<ISettingEditor> GetEnumerator()
            => SettingEditors.GetEnumerator();

        public ISettingEditorCollection<TSetting> WithDescription(string description)
            => MiscUtil.InlineAction(this, o => o.Description = description);
        public ISettingEditorCollection<TSetting> WithName(string name)
            => MiscUtil.InlineAction(this, o => o.Name = name);
        public ISettingEditorCollection<TSetting> WithNotes(string notes)
            => MiscUtil.InlineAction(this, o => o.Notes = notes);

        IEnumerator IEnumerable.GetEnumerator()
            => SettingEditors.GetEnumerator();

        ISettingEditorCollection ISettingEditorCollection.WithDescription(string description)
            => MiscUtil.InlineAction(this, o => o.Description = description);
        ISettingEditorCollection ISettingEditorCollection.WithName(string name)
            => MiscUtil.InlineAction(this, o => o.Name = name);
        ISettingEditorCollection ISettingEditorCollection.WithNotes(string notes)
            => MiscUtil.InlineAction(this, o => o.Notes = notes);

        private string GetName<TStore>(Expression<Func<TSetting, TStore>> property)
            => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";
    }
}
