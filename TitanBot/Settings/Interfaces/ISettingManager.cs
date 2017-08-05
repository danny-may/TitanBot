using Discord;
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

        IReadOnlyList<ISettingEditorCollection> GetEditors(SettingScope scope);
        ISettingEditorCollection<T> GetEditorCollection<T>(SettingScope scope);
    }
}
