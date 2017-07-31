using System;
using System.Linq.Expressions;
using System.Reflection;
using TitanBot.Commands;
using TitanBot.Util;

namespace TitanBot.Settings
{
    class SettingEditorBuilder<TSetting, TStore, TAccept> : ISettingEditorBuilder<TStore, TAccept>
    {
        public string Name { get; private set; }
        public string[] Aliases { get; private set; }
        public Func<ICommandContext, TAccept, TStore> Converter { get; }
        public Func<ICommandContext, TAccept, string> Validator { get; private set; }
        public Func<ICommandContext, TStore, string> Viewer { get; private set; }
        private Expression<Func<TSetting, TStore>> Property { get; }
        private ISettingManager Parent { get; }


        public SettingEditorBuilder(ISettingManager parent, Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TAccept, TStore> converter)
        {
            Parent = parent;
            Property = property;
            Converter = converter;
        }

        public ISettingEditor Build()
        {
            if (Converter == null)
                throw new InvalidOperationException($"No method of conversion from {typeof(TAccept)} to {typeof(TStore)} has been specified!");
            return new SettingEditor<TSetting, TStore, TAccept>(e => Parent.GetContext(e).Get<TSetting>(), (e, s) => Parent.GetContext(e).Edit(s),
                                                            string.IsNullOrWhiteSpace(Name) ? GetName(Property) : Name, Aliases, Property, Converter, Viewer, Validator);
        }

        public ISettingEditorBuilder<TStore, TAccept> SetName(string name)
            => MiscUtil.InlineAction(this, o => o.Name = name);

        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<ICommandContext, TAccept, string> validator)
            => MiscUtil.InlineAction(this, o => o.Validator = validator);
        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, string> validator)
            => MiscUtil.InlineAction(this, o => o.Validator = (c, a) => validator(a));

        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<ICommandContext, TStore, string> viewer)
            => MiscUtil.InlineAction(this, o => o.Viewer = viewer);
        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, string> viewer)
            => MiscUtil.InlineAction(this, o => o.Viewer = (c, a) => viewer(a));

        private string GetName(Expression<Func<TSetting, TStore>> property)
            => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";

        public ISettingEditorBuilder<TStore, TAccept> SetAlias(params string[] aliases)
            => MiscUtil.InlineAction(this, o => o.Aliases = aliases);
    }
}
