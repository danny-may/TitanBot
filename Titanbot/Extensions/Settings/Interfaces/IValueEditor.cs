using System;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Interfaces
{
    public interface IValueEditor
    {
        string Name { get; }
        string[] Aliases { get; }
        bool AllowGroups { get; }
        Type MemberType { get; }
        bool TrySet(ISettingEditorContext context, ulong id, string group, object value, out IDisplayable<string> errors);
        IDisplayable<string> Display(ISettingEditorContext context, ulong id, string group);
        object Get(ulong id, string group);
        T Get<T>(ulong id, string group);
    }
}