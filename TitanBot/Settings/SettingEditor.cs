using Discord;
using System;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    class SettingEditor<TSetting, TStore, TAccept> : ISettingEditor
    {
        Func<IEntity<ulong>, TSetting> Getter { get; }
        Action<IEntity<ulong>, Action<TSetting>> Editor { get; }
        Func<ICommandContext, TAccept, TStore> Converter { get; }
        Func<ICommandContext, TStore, string> Viewer { get; }
        Func<ICommandContext, TAccept, string> Validator { get; }

        Action<TSetting, TStore> Setter { get; }
        Func<TSetting, TStore> Value { get; }

        public string Name { get; }
        public Type Type => typeof(TAccept);

        public SettingEditor(Func<IEntity<ulong>, TSetting> getter, 
                             Action<IEntity<ulong>, Action<TSetting>> editor, 
                             string name,
                             Expression<Func<TSetting, TStore>> property,
                             Func<ICommandContext, TAccept, TStore> converter,
                             Func<ICommandContext, TStore, string> viewer,
                             Func<ICommandContext, TAccept, string> validator)
        {
            Name = name;
            Getter = getter;
            Editor = editor;
            Converter = converter;
            Viewer = viewer ?? ((c, s) => s?.ToString());
            Validator = validator ?? ((c, s) => null);

            Setter = CreateSetter(property);
            Value = property.Compile();
        }

        public string Display(ICommandContext context, IEntity<ulong> entity)
            => Viewer(context, Value(Getter(entity)));

        public bool TrySet(ICommandContext context, IEntity<ulong> entity, object value, out string error)
            => TrySet(context, entity, (TAccept)value, out error);

        private bool TrySet(ICommandContext context, IEntity<ulong> entity, TAccept value, out string error)
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
