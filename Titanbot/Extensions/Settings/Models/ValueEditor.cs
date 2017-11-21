using System;
using System.Linq;
using System.Linq.Expressions;
using Titanbot.Extensions.Settings.Interfaces;
using Titansmasher.Services.Display;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Models
{
    internal class ValueEditor<TSetting, TStore, TAccept> : IValueEditor
    {
        #region Fields

        private Func<ulong, string, TSetting> SettingGetter { get; }
        private Action<ulong, string, TSetting> Editor { get; }
        private Func<TAccept, TStore> Converter { get; }
        private Func<ISettingEditorContext, TStore, IDisplayable<string>> Viewer { get; }
        private Func<ISettingEditorContext, TAccept, IDisplayable<string>> Validator { get; }

        private Action<TSetting, TStore> Setter { get; }
        private Func<TSetting, TStore> Getter { get; }

        #endregion Fields

        #region Constructors

        public ValueEditor(IValueEditorBuilder<TSetting, TStore, TAccept> builder)
        {
            Name = builder.Name;
            Aliases = builder.Aliases;
            SettingGetter = (i, g) => builder.Parent.Parent.Setttings.GroupsFor(i).GetGroup(g).Get<TSetting>();
            Editor = (i, g, s) => builder.Parent.Parent.Setttings.GroupsFor(i).GetGroup(g).Update(s);
            AllowGroups = builder.Groups;
            Converter = builder.Converter;
            Viewer = builder.Viewer;
            Validator = builder.Validator;

            Setter = CreateSetter(builder.Property);
            Getter = builder.Property.Compile();
        }

        #endregion Constructors

        #region Methods

        private Action<TSetting, TStore> CreateSetter(Expression<Func<TSetting, TStore>> selector)
        {
            var valueParam = Expression.Parameter(typeof(TStore));
            var body = Expression.Assign(selector.Body, valueParam);
            return Expression.Lambda<Action<TSetting, TStore>>(body,
                selector.Parameters.Single(),
                valueParam).Compile();
        }

        private bool GroupAllowed(string group)
            => AllowGroups || group == null;

        #endregion Methods

        #region IValueEditor

        public string Name { get; }
        public string[] Aliases { get; }
        public bool AllowGroups { get; }
        public Type MemberType { get; }

        public IDisplayable<string> Display(ISettingEditorContext context, ulong id, string group)
        {
            if (GroupAllowed(group))
                return Viewer(context, Get<TStore>(id, group));
            return null;
        }

        public object Get(ulong id, string group)
        {
            if (GroupAllowed(group))
                return Getter(SettingGetter(id, group));
            return null;
        }

        public T Get<T>(ulong id, string group)
            => Get(id, group) is T v ? v : default;

        public bool TrySet(ISettingEditorContext context, ulong id, string group, object value, out IDisplayable<string> errors)
        {
            if (!GroupAllowed(group))
            {
                errors = new Translation("settings.editor.nogroups", Name, group);
                return false;
            }
            else if (!(value is TAccept accept))
            {
                errors = new Translation("settings.editor.typemismatch", Name, value?.GetType(), typeof(TAccept));
                return false;
            }
            else
            {
                errors = Validator(context, accept);
                if (errors != null)
                    return false;

                var setting = SettingGetter(id, group);
                Setter(setting, Converter(accept));
                Editor(id, group, setting);
                return true;
            }
        }

        #endregion IValueEditor
    }
}