using System;
using System.Linq;
using System.Linq.Expressions;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    public abstract class EditableSetting
    {
        public string Name { get; protected set; }
        public abstract Type Type { get; }
        public abstract bool TrySave(ICommandContext context, ulong guildId, object value, out string errors);
        public abstract string Display(ICommandContext context, ulong guildId);

        public static EditableSetting Create<TGroup, TStore, TAccept>(Func<ulong, TGroup> retriever,
                                                                Action<ulong, TGroup> saver,
                                                                string name,
                                                                Expression<Func<TGroup, TStore>> property,
                                                                Func<ICommandContext, TAccept, TStore> converter,
                                                                Func<ICommandContext, TStore, string> viewer,
                                                                Func<ICommandContext, TAccept, string> validator)
            => new TypedSetting<TGroup, TStore, TAccept>(retriever, saver, name, property, converter, viewer, validator);

        private class TypedSetting<TGroup, TStore, TAccept> : EditableSetting
        {
            Func<ulong, TGroup> Retriever { get; }
            Action<ulong, TGroup> Saver { get; }
            Func<ICommandContext, TAccept, TStore> Converter { get; }
            Func<ICommandContext, TStore, string> Viewer { get; }
            Func<ICommandContext, TAccept, string> Validator { get; }
            Action<TGroup, TStore> Setter { get; }
            Func<TGroup, TStore> Getter { get; }

            public override Type Type => typeof(TAccept);

            internal TypedSetting(Func<ulong, TGroup> retriever,
                                  Action<ulong, TGroup> saver,
                                  string name,
                                  Expression<Func<TGroup, TStore>> property,
                                  Func<ICommandContext, TAccept, TStore> converter,
                                  Func<ICommandContext, TStore, string> viewer,
                                  Func<ICommandContext, TAccept, string> validator)
            {
                Name = name;
                Retriever = retriever;
                Setter = CreateSetter(property);
                Getter = property.Compile();
                Converter = converter;
                Viewer = viewer ?? ((c, s) => s?.ToString());
                Validator = validator ?? ((c, s) => null);
                Saver = saver;
            }

            public override string Display(ICommandContext context, ulong guildId)
                => Viewer(context, Getter(Retriever(guildId)));

            public override bool TrySave(ICommandContext context, ulong guildId, object value, out string errors)
            {
                errors = Validator(context, (TAccept)value);

                if (errors != null)
                    return false;

                var group = Retriever(guildId);
                Setter(group, Converter(context, (TAccept)value));
                Saver(guildId, group);
                return true;
            }

            private Action<TGroup, TStore> CreateSetter(Expression<Func<TGroup, TStore>> selector)
            {
                var valueParam = Expression.Parameter(typeof(TStore));
                var body = Expression.Assign(selector.Body, valueParam);
                return Expression.Lambda<Action<TGroup, TStore>>(body,
                    selector.Parameters.Single(),
                    valueParam).Compile();
            }
        }
    }
}
