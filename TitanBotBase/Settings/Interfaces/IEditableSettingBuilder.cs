using System;
using System.Linq.Expressions;

namespace TitanBotBase.Settings
{
    public interface IEditableSettingBuilder<TGroup> : IDisposable
    {
        IEditableSettingBuilder<TGroup> AddSetting<TAccept, TStore>(Expression<Func<TGroup, TStore>> property, Func<TAccept, TStore> converter, Func<TStore, string> viewer = null, Func<TAccept, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore, TAccept>(string name, Expression<Func<TGroup, TStore>> property, Func<TAccept, TStore> converter, Func<TStore, string> viewer = null, Func<TAccept, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore>(Expression<Func<TGroup, TStore>> property, Func<TStore, string> viewer = null, Func<TStore, string> validator = null);
        IEditableSettingBuilder<TGroup> AddSetting<TStore>(string name, Expression<Func<TGroup, TStore>> property, Func<TStore, string> viewer = null, Func<TStore, string> validator = null);
        void Finalise();
        IEditableSettingBuilder<TGroup> WithDescription(string description);
        IEditableSettingBuilder<TGroup> WithName(string name);
    }
}