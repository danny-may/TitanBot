using System;
using TitanBot.Core.Models;

namespace TitanBot.Core.Services.Setting
{
    public interface ISettingCollection
    {
        ulong ContextId { get; }

        T GetModel<T>() where T : new();
        void SaveModel<T>(T model) where T : new();
        void EditModel<T>(Action<T> edit) where T : new();

        T Get<T>(string key);
        void Save<T>(string key, T value);
        void Edit<T>(string key, Action<T> edit) where T : new();

        void BindTo<T>(T model) where T : BoundModel;
        void UnBind<T>(T model) where T : BoundModel;
    }
}