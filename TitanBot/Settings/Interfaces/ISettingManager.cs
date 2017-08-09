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

        IReadOnlyList<ISettingEditorCollection> GetEditors(SettingScope scope);
        ISettingEditorCollection<T> GetEditorCollection<T>(SettingScope scope);
    }
}
