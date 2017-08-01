using Discord;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TitanBot.Contexts;

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
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<IMessageContext, TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> builder = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> builder = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Action<ISettingEditorBuilder<TStore, TStore>> builder = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<IMessageContext, TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> builder = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<TAccept, TStore> converter, Action<ISettingEditorBuilder<TStore, TAccept>> builder = null);
        ISettingEditorCollection<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore[]>> property, Action<ISettingEditorBuilder<TStore, TStore>> builder = null);

        //Entity Stuff
        ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong>> property, Action<ISettingEditorBuilder<ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>;
        ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong?>> property, Action<ISettingEditorBuilder<ulong?, TAccept>> builder = null) where TAccept : IEntity<ulong>;
        ISettingEditorCollection<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong[]>> property, Action<ISettingEditorBuilder<ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>;

        new ISettingEditorCollection<TSetting> WithName(string name);
        new ISettingEditorCollection<TSetting> WithDescription(string description);
        new ISettingEditorCollection<TSetting> WithNotes(string notes);
    }
}
