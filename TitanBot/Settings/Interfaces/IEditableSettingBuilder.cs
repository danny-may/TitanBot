using System;
using System.Linq.Expressions;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    public interface IEditableSettingBuilder<TGroup> : IDisposable
    {
        IEditableSettingBuilder<TGroup> AddSetting<TAccept, TStore>(Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore, TAccept>(string name, Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TAccept, TStore> converter, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TAccept, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore>(Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore>(string name, Expression<Func<TGroup, TStore>> property, Func<ICommandContext, TStore, string> viewer = null, Func<ICommandContext, TStore, string> validator = null);
        void Finalise();
        IEditableSettingBuilder<TGroup> WithDescription(string description);
        IEditableSettingBuilder<TGroup> WithName(string name);
        IEditableSettingBuilder<TGroup> WithNotes(string notes);
    }
}