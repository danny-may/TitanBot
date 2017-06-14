using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Dependencies;
using TitanBotBase.TypeReaders;

namespace TitanBotBase.Settings
{
    public class EditableSettingBuilder<TGroup> : IEditableSettingBuilder<TGroup>
    {
        string Name { get; set; } = typeof(TGroup).Name;
        string Description { get; set; }
        List<EditableSetting> Settings { get; } = new List<EditableSetting>();
        bool hasFinalised = false;
        Dictionary<Type, IEditableSettingGroup> Groups { get; }
        IDependencyFactory DependencyFactory { get; }

        internal EditableSettingBuilder(Dictionary<Type, IEditableSettingGroup> groups, IDependencyFactory factory)
        {
            Groups = groups;
            DependencyFactory = factory;
        }

        public void Finalise()
        {
            if (hasFinalised)
                throw new InvalidOperationException("You cannot call Finalise more than once per instance");
            hasFinalised = true;
            Groups[typeof(TGroup)] = DependencyFactory.WithInstance(typeof(TGroup))
                                                      .WithInstance(Settings)
                                                      .Construct<IEditableSettingGroup>()
                                                      .WithDescription(Description)
                                                      .WithName(Name);
        }

        public IEditableSettingBuilder<TGroup> WithName(string name)
        {
            Name = name;
            return this;
        }

        public IEditableSettingBuilder<TGroup> WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public void Dispose()
        {
            if (!hasFinalised)
                Finalise();
        }

        private string GetName<TStore>(Expression<Func<TGroup, TStore>> property)
            => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";

        public IEditableSettingBuilder<TGroup> AddSetting<TStore, TAccept>(string name, Expression<Func<TGroup, TStore>> property, Func<TAccept, TStore> converter, Func<TStore, string> viewer = null, Func<TAccept, string> validator = null)
        {
            Settings.Add(EditableSetting.Create(name, property, converter, viewer, validator));
            return this;
        }
        public IEditableSettingBuilder<TGroup> AddSetting<TAccept, TStore>(Expression<Func<TGroup, TStore>> property, Func<TAccept, TStore> converter, Func<TStore, string> viewer = null, Func<TAccept, string> validator = null)
            => AddSetting(GetName(property), property, converter, viewer, validator);
        public IEditableSettingBuilder<TGroup> AddSetting<TStore>(Expression<Func<TGroup, TStore>> property, Func<TStore, string> viewer = null, Func<TStore, string> validator = null)
            => AddSetting(GetName(property), property, viewer, validator);
        public IEditableSettingBuilder<TGroup> AddSetting<TStore>(string name, Expression<Func<TGroup, TStore>> property, Func<TStore, string> viewer = null, Func<TStore, string> validator = null)
            => AddSetting(name, property, v => v, viewer, validator);
    }

    class EditableSettingGroup : IEditableSettingGroup
    {
        public string GroupName { get; private set; }
        public string Description { get; private set; }
        public List<EditableSetting> Settings { get; }
        public Type GroupType { get; }

        internal EditableSettingGroup(Type groupType, IEnumerable<EditableSetting> settings)
        {
            GroupType = groupType;
            GroupName = GroupType.Name;
            Settings = settings.ToList();
        }

        public IEditableSettingGroup WithName(string name)
        {
            GroupName = name;
            return this;
        }
        public IEditableSettingGroup WithDescription(string description)
        {
            Description = description;
            return this;
        }
    }

    public abstract class EditableSetting
    {
        public string Name { get; protected set; }
        public abstract Type Type { get; }
        public abstract bool TrySave(ISettingsManager manager, ulong guildId, object value, out string errors);
        public abstract string Display(ISettingsManager manager, ulong guildId);

        public static EditableSetting Create<TGroup, TStore, TAccept>(string name,
                                                                Expression<Func<TGroup, TStore>> property,
                                                                Func<TAccept, TStore> converter,
                                                                Func<TStore, string> viewer,
                                                                Func<TAccept, string> validator)
            => new TypedSetting<TGroup, TStore, TAccept>(name, property, converter, viewer, validator);

        private class TypedSetting<TGroup, TStore, TAccept> : EditableSetting
        {
            Func<TAccept, TStore> Converter { get; }
            Func<TStore, string> Viewer { get; }
            Func<TAccept, string> Validator { get; }
            Action<TGroup, TStore> Setter { get; }
            Func<TGroup, TStore> Getter { get; }

            public override Type Type => typeof(TAccept);

            internal TypedSetting(string name,
                                  Expression<Func<TGroup, TStore>> property,
                                  Func<TAccept, TStore> converter,
                                  Func<TStore, string> viewer,
                                  Func<TAccept, string> validator)
            {
                Name = name;
                Setter = CreateSetter(property);
                Getter = property.Compile();
                Converter = converter;
                Viewer = viewer ?? (s => s?.ToString());
                Validator = validator ?? (s => null);
            }

            public override string Display(ISettingsManager manager, ulong guildId)
                => Viewer(Getter(manager.GetGroup<TGroup>(guildId)));

            public override bool TrySave(ISettingsManager manager, ulong guildId, object value, out string errors)
            {
                if (!(value is TAccept))
                    errors = $"`{value.GetType()}` is not a valid {typeof(TAccept)}";
                else
                    errors = Validator((TAccept)value);

                if (errors != null)
                    return false;

                var group = manager.GetGroup<TGroup>(guildId);
                Setter(group, Converter((TAccept)value));
                manager.SaveGroup(guildId, group);
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
