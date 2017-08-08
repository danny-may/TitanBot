using Discord;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TitanBot.Contexts;
using TitanBot.Formatting.Interfaces;
using TitanBot.Replying;

namespace TitanBot.Settings
{
    public interface ISettingEditorCollection : IReadOnlyList<ISettingEditor>
    {
        string Name { get; }
        ILocalisable<string> Description { get; }
        ILocalisable<string> Notes { get; }
        ISettingManager Parent { get; }

        ISettingEditorCollection WithName(string name);
        ISettingEditorCollection WithDescription(ILocalisable<string> description);
        ISettingEditorCollection WithNotes(ILocalisable<string> notes);
        ISettingEditorCollection WithRawDescription(string text);
        ISettingEditorCollection WithRawNotes(string text);
        ISettingEditorCollection WithDescription(string key);
        ISettingEditorCollection WithNotes(string key);
        ISettingEditorCollection WithDescription(string key, ReplyType replyType);
        ISettingEditorCollection WithNotes(string key, ReplyType replyType);
        ISettingEditorCollection WithDescription(string key, params object[] values);
        ISettingEditorCollection WithNotes(string key, params object[] values);
        ISettingEditorCollection WithDescription(string key, ReplyType replyType, params object[] values);
        ISettingEditorCollection WithNotes(string key, ReplyType replyType, params object[] values);
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
        new ISettingEditorCollection<TSetting> WithDescription(ILocalisable<string> description);
        new ISettingEditorCollection<TSetting> WithNotes(ILocalisable<string> notes);
        new ISettingEditorCollection<TSetting> WithRawDescription(string text);
        new ISettingEditorCollection<TSetting> WithRawNotes(string text);
        new ISettingEditorCollection<TSetting> WithDescription(string key);
        new ISettingEditorCollection<TSetting> WithNotes(string key);
        new ISettingEditorCollection<TSetting> WithDescription(string key, ReplyType replyType);
        new ISettingEditorCollection<TSetting> WithNotes(string key, ReplyType replyType);
        new ISettingEditorCollection<TSetting> WithDescription(string key, params object[] values);
        new ISettingEditorCollection<TSetting> WithNotes(string key, params object[] values);
        new ISettingEditorCollection<TSetting> WithDescription(string key, ReplyType replyType, params object[] values);
        new ISettingEditorCollection<TSetting> WithNotes(string key, ReplyType replyType, params object[] values);
    }
}
