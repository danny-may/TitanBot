using System;
using System.Linq.Expressions;
using Titanbot.Extensions.Settings.Interfaces;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Models
{
    internal class ValueEditorBuilder<TSetting, TStore, TAccept> : IValueEditorBuilder<TSetting, TStore, TAccept>
    {
        #region Constructors

        public ValueEditorBuilder(ISettingEditor parent, Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        #endregion Constructors

        #region IValueEditorBuilder

        public string Name { get; set; }
        public bool Groups { get; set; }
        public string[] Aliases { get; set; }
        public Func<ISettingEditorContext, TStore, IDisplayable<string>> Viewer { get; set; }
        public Func<TAccept, TStore> Converter { get; }
        public Func<ISettingEditorContext, TAccept, IDisplayable<string>> Validator { get; set; }
        public Expression<Func<TSetting, TStore>> Property { get; }
        public ISettingEditor Parent { get; }

        public IValueEditor Build()
            => new ValueEditor<TSetting, TStore, TAccept>(this);

        public IValueEditorBuilder<TSetting, TStore, TAccept> AllowGroups(bool allow = true)
        {
            Groups = allow;
            return this;
        }

        public IValueEditorBuilder<TSetting, TStore, TAccept> SetAlias(params string[] aliases)
        {
            Aliases = aliases;
            return this;
        }

        public IValueEditorBuilder<TSetting, TStore, TAccept> SetName(string name)
        {
            Name = name;
            return this;
        }

        public IValueEditorBuilder<TSetting, TStore, TAccept> SetValidator(Func<ISettingEditorContext, TAccept, IDisplayable<string>> validator)
        {
            Validator = validator;
            return this;
        }

        public IValueEditorBuilder<TSetting, TStore, TAccept> SetViewer(Func<ISettingEditorContext, TStore, IDisplayable<string>> viewer)
        {
            Viewer = viewer;
            return this;
        }

        #endregion IValueEditorBuilder
    }
}