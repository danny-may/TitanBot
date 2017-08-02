using System;
using System.Linq.Expressions;
using System.Reflection;
using TitanBot.Contexts;
using TitanBot.Util;

namespace TitanBot.Settings
{
    class SettingEditorBuilder<TSetting, TStore, TAccept> : ISettingEditorBuilder<TStore, TAccept>
    {
        public string Name { get; private set; }
        public string[] Aliases { get; private set; }
        public Func<IMessageContext, TAccept, TStore> Converter { get; }
        public Func<IMessageContext, TAccept, string> Validator { get; private set; }
        public Func<IMessageContext, TStore, string> Viewer { get; private set; }
        private Expression<Func<TSetting, TStore>> Property { get; }
        private ISettingManager Parent { get; }


        public SettingEditorBuilder(ISettingManager parent, Expression<Func<TSetting, TStore>> property, Func<IMessageContext, TAccept, TStore> converter)
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
        {
            Name = name;
            return this;
        }

        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, string> validator)
            => SetValidator((c, a) => validator(a));
        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<IMessageContext, TAccept, string> validator)
        {
            Validator = validator;
            return this;
        }

        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, string> viewer)
            => SetViewer((c, s) => viewer(s));
        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<IMessageContext, TStore, string> viewer)
        {
            Viewer = viewer;
            return this;
        }

        private string GetName(Expression<Func<TSetting, TStore>> property)
            => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";

        public ISettingEditorBuilder<TStore, TAccept> SetAlias(params string[] aliases)
        {
            Aliases = aliases;
            return this;
        }
    }
}
