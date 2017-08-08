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
        Func<IEntity<ulong>, TSetting> SettingGetter { get; }
        Action<IEntity<ulong>, Action<TSetting>> Editor { get; }
        Func<IMessageContext, TAccept, TStore> Converter { get; }
        Func<IMessageContext, TStore, ILocalisable<string>> Viewer { get; }
        Func<IMessageContext, TAccept, ILocalisable<string>> Validator { get; }

        Action<TSetting, TStore> Setter { get; }
        Func<TSetting, TStore> Getter { get; }

        public string Name { get; }
        public string[] Aliases { get; }
        public Type Type => typeof(TAccept);

        public SettingEditor(Func<IEntity<ulong>, TSetting> getter, 
                             Action<IEntity<ulong>, Action<TSetting>> editor, 
                             string name,
                             string[] aliases,
                             Expression<Func<TSetting, TStore>> property,
                             Func<IMessageContext, TAccept, TStore> converter,
                             Func<IMessageContext, TStore, ILocalisable<string>> viewer,
                             Func<IMessageContext, TAccept, ILocalisable<string>> validator)
        {
            Name = name;
            Aliases = aliases;
            SettingGetter = getter;
            Editor = editor;
            Converter = converter;
            Viewer = viewer ?? ((c, s) => (RawString)s?.ToString());
            Validator = validator ?? ((c, s) => null);

            Setter = CreateSetter(property);
            Getter = property.Compile();
        }

        public object Get(IEntity<ulong> entity)
            => Getter(SettingGetter(entity));

        public ILocalisable<string> Display(ICommandContext context, IEntity<ulong> entity)
            => Viewer(context, (TStore)Get(entity));

        public bool TrySet(ICommandContext context, IEntity<ulong> entity, object value, out ILocalisable<string> error)
            => TrySet(context, entity, (TAccept)value, out error);

        private bool TrySet(ICommandContext context, IEntity<ulong> entity, TAccept value, out ILocalisable<string> error)
        {
            error = Validator(context, value);
            if (error != null)
                return false;

            Editor(entity, s => Setter(s, Converter(context, value)));
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
