using System;
using System.Linq.Expressions;
using System.Reflection;
using TitanBot.Contexts;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Settings
{
    internal class SettingEditorBuilder<TSetting, TStore, TAccept> : ISettingEditorBuilder<TStore, TAccept>
    {
        public string Name { get; private set; }
        public bool Groups { get; private set; }
        public string[] Aliases { get; private set; }
        public Func<IMessageContext, TAccept, TStore> Converter { get; }
        public Func<IMessageContext, TAccept, ILocalisable<string>> Validator { get; private set; }
        public Func<IMessageContext, TStore, ILocalisable<string>> Viewer { get; private set; }
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
            return new SettingEditor<TSetting, TStore, TAccept>((e, g) => Parent.GetContext(e).Get<TSetting>(g), (e, g, s) => Parent.GetContext(e).Edit(g, s),
                                                            string.IsNullOrWhiteSpace(Name) ? GetName(Property) : Name, Aliases, Property, Converter, Viewer, Validator, Groups);
        }

        public ISettingEditorBuilder<TStore, TAccept> SetName(string name)
        {
            Name = name;
            return this;
        }

        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, ILocalisable<string>> validator)
            => SetValidator((c, a) => validator(a));

        public ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<IMessageContext, TAccept, ILocalisable<string>> validator)
        {
            Validator = validator;
            return this;
        }

        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, ILocalisable<string>> viewer)
            => SetViewer((c, s) => viewer(s));

        public ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<IMessageContext, TStore, ILocalisable<string>> viewer)
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

        ISettingEditorBuilder<TStore, TAccept> ISettingEditorBuilder<TStore, TAccept>.AllowGroups(bool allow)
        {
            Groups = allow;
            return this;
        }
    }
}