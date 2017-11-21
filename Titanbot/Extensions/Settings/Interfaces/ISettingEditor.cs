using Discord;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Interfaces
{
    public interface ISettingEditor
    {
        string Name { get; set; }
        IDisplayable<string> Description { get; set; }
        IDisplayable<string> Notes { get; set; }
        ISettingEditorManager Parent { get; }

        IReadOnlyList<IValueEditor> ValueEditors { get; }

        ISettingEditor SetName(string name);
        ISettingEditor SetDescription(IDisplayable<string> description);
        ISettingEditor SetNotes(IDisplayable<string> notes);
    }

    public interface ISettingEditor<TSetting> : ISettingEditor
        where TSetting : class, new()
    {
        //Single settings
        ISettingEditor<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore>> property, Func<TAccept, TStore> converter, Action<IValueEditorBuilder<TSetting, TStore, TAccept>> builder = null);
        ISettingEditor<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore>> property, Action<IValueEditorBuilder<TSetting, TStore, TStore>> builder = null);

        //Array settings
        ISettingEditor<TSetting> AddSetting<TStore, TAccept>(Expression<Func<TSetting, TStore[]>> property, Func<TAccept, TStore> converter, Action<IValueEditorBuilder<TSetting, TStore, TAccept>> builder = null);
        ISettingEditor<TSetting> AddSetting<TStore>(Expression<Func<TSetting, TStore[]>> property, Action<IValueEditorBuilder<TSetting, TStore, TStore>> builder = null);

        //Entity Stuff
        ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong>> property, Action<IValueEditorBuilder<TSetting, ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>;
        ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong?>> property, Action<IValueEditorBuilder<TSetting, ulong?, TAccept>> builder = null) where TAccept : IEntity<ulong>;
        ISettingEditor<TSetting> AddSetting<TAccept>(Expression<Func<TSetting, ulong[]>> property, Action<IValueEditorBuilder<TSetting, ulong, TAccept>> builder = null) where TAccept : IEntity<ulong>;

        new ISettingEditor<TSetting> SetName(string name);
        new ISettingEditor<TSetting> SetDescription(IDisplayable<string> description);
        new ISettingEditor<TSetting> SetNotes(IDisplayable<string> notes);
    }
}