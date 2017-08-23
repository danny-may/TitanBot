using Discord;
using System;
using System.Collections.Generic;

namespace TitanBot.Settings
{
    public interface ISettingManager
    {
        IEntity<ulong> Global { get; }

        ISettingContext GetContext(IEntity<ulong> entity);
        ISettingContext GetContext(ulong entity);
        void ResetSettings(IEntity<ulong> entity);
        void ResetSettings(ulong entity);

        void Migrate(Dictionary<string, Type> typeMap);

        IReadOnlyDictionary<int, string[]> GetGroups(ulong entity);
        IReadOnlyDictionary<int, string[]> GetGroups(IEntity<ulong> entity);
        void SetGroup(ulong entity, int value, string[] keys);
        void SetGroup(IEntity<ulong> entity, int value, string[] keys);
        void RemoveGroup(ulong entity, int value);
        void RemoveGroup(IEntity<ulong> entity, int value);

        IReadOnlyList<ISettingEditorCollection> GetEditors(SettingScope scope);
        ISettingEditorCollection<T> GetEditorCollection<T>(SettingScope scope);
    }
}
