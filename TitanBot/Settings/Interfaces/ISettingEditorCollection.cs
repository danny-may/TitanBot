using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    public interface ISettingEditorCollection : IReadOnlyList<ISettingEditor>
    {
        string Name { get; }
        string Description { get; }
        string Notes { get; }
        ISettingManager Parent { get; }

        ISettingEditorCollection WithName(string name);
        ISettingEditorCollection WithDescription(string description);
        ISettingEditorCollection WithNotes(string notes);
    }
    public interface ISettingEditorCollection<TSetting> : ISettingEditorCollection
    {
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(string name, Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore>(string name, Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null);

        //ISettingEditorCollection<TSetting> AddEntitySetting<TEntity>(string name, Expression<Func<TSetting, ulong?>> property, )


        new ISettingEditorCollection<TSetting> WithName(string name);
        new ISettingEditorCollection<TSetting> WithDescription(string description);
        new ISettingEditorCollection<TSetting> WithNotes(string notes);
    }
}
