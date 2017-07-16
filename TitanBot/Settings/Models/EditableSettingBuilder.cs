using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TitanBot.Commands;
using TitanBot.Dependencies;

namespace TitanBot.Settings
{
    public class EditableSettingBuilder<TGroup> : IEditableSettingBuilder<TGroup>
    {
        string Name { get; set; } = typeof(TGroup).Name;
        string Description { get; set; }
        string Notes { get; set; }
        List<EditableSetting> Settings { get; } = new List<EditableSetting>();
        bool hasFinalised = false;
        Dictionary<Type, IEditableSettingGroup> Groups { get; }
        IDependencyFactory DependencyFactory { get; }
        Func<ulong, TGroup> Retriever { get; }
        Action<ulong, TGroup> Saver { get; }

        internal EditableSettingBuilder(Dictionary<Type, IEditableSettingGroup> groups, IDependencyFactory factory, Func<ulong, TGroup> retriever, Action<ulong, TGroup> saver)
        {
            Groups = groups;
            DependencyFactory = factory;
            Retriever = retriever;
            Saver = saver;
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
                                                      .WithName(Name)
                                                      .WithNotes(Notes);
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

        public IEditableSettingBuilder<TGroup> WithNotes(string notes)
        {
            Notes = notes;
            return this;
        }

        public void Dispose()
        {
            if (!hasFinalised)
                Finalise();
        }

        private string GetName<TStore>(Expression<Func<TGroup, TStore>> property)
            => ((property.Body as MemberExpression)?.Member as PropertyInfo)?.Name ?? "UNKOWN_PROPERTYNAME";

        public IEditableSettingBuilder<TGroup> AddSetting<TStore, TAccept>(string name, Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null)
        {
            Settings.Add(EditableSetting.Create(Retriever, Saver, name, property, converter, viewer, validator));
            return this;
        }
        public IEditableSettingBuilder<TGroup> AddSetting<TAccept, TStore>(Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null)
            => AddSetting(GetName(property), property, converter, viewer, validator);
        public IEditableSettingBuilder<TGroup> AddSetting<TStore>(Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null)
            => AddSetting(GetName(property), property, viewer, validator);
        public IEditableSettingBuilder<TGroup> AddSetting<TStore>(string name, Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null)
            => AddSetting(name, property, (c, v) => v, viewer, validator);
    }
}
