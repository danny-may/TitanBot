using Discord;
using System;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Contexts;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Settings
{
    class SettingEditor<TSetting, TStore, TAccept> : ISettingEditor
    {
        Func<IEntity<ulong>, int, TSetting> SettingGetter { get; }
        Action<IEntity<ulong>, int, Action<TSetting>> Editor { get; }
        Func<IMessageContext, TAccept, TStore> Converter { get; }
        Func<IMessageContext, TStore, ILocalisable<string>> Viewer { get; }
        Func<IMessageContext, TAccept, ILocalisable<string>> Validator { get; }

        Action<TSetting, TStore> Setter { get; }
        Func<TSetting, TStore> Getter { get; }

        public string Name { get; }
        public string[] Aliases { get; }
        public Type Type => typeof(TAccept);
        public bool AllowGroups { get; }

        public SettingEditor(Func<IEntity<ulong>, int, TSetting> getter,
                             Action<IEntity<ulong>, int, Action<TSetting>> editor,
                             string name,
                             string[] aliases,
                             Expression<Func<TSetting, TStore>> property,
                             Func<IMessageContext, TAccept, TStore> converter,
                             Func<IMessageContext, TStore, ILocalisable<string>> viewer,
                             Func<IMessageContext, TAccept, ILocalisable<string>> validator,
                             bool allowGroups)
        {
            Name = name;
            Aliases = aliases;
            SettingGetter = getter;
            AllowGroups = allowGroups;
            Editor = editor;
            Converter = converter;
            Viewer = viewer ?? ((c, s) => (RawString)s?.ToString());
            Validator = validator ?? ((c, s) => null);

            Setter = CreateSetter(property);
            Getter = property.Compile();
        }

        private bool CheckGroup(int group)
            => !(AllowGroups || group == 0) ? throw new InvalidOperationException("This editable setting does not permit groups") : true;

        public object Get(IEntity<ulong> entity, int group)
        {
            if (CheckGroup(group))
                return Getter(SettingGetter(entity, group));
            return null;
        }

        public ILocalisable<string> Display(ICommandContext context, IEntity<ulong> entity, int group)
        {
            if (CheckGroup(group))
                return Viewer(context, (TStore)Get(entity, group));
            return null;
        }

        public bool TrySet(ICommandContext context, IEntity<ulong> entity, int group, object value, out ILocalisable<string> error)
        {
            error = null;
            if (CheckGroup(group))
                return TrySet(context, entity, group, (TAccept)value, out error);
            return false;
        }

        private bool TrySet(ICommandContext context, IEntity<ulong> entity, int group, TAccept value, out ILocalisable<string> error)
        {
            error = Validator(context, value);
            if (error != null)
                return false;

            Editor(entity, group, s => Setter(s, Converter(context, value)));
            return true;
        }

        private Action<TSetting, TStore> CreateSetter(Expression<Func<TSetting, TStore>> selector)
        {
            var valueParam = Expression.Parameter(typeof(TStore));
            var body = Expression.Assign(selector.Body, valueParam);
            return Expression.Lambda<Action<TSetting, TStore>>(body,
                selector.Parameters.Single(),
                valueParam).Compile();
        }
    }
}
