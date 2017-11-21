using System;
using System.Linq.Expressions;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Extensions.Settings.Interfaces
{
    public interface IValueEditorBuilder<TSetting, TStore, TAccept>
    {
        string Name { get; set; }
        bool Groups { get; set; }
        string[] Aliases { get; set; }
        Func<ISettingEditorContext, TStore, IDisplayable<string>> Viewer { get; set; }
        Func<TAccept, TStore> Converter { get; }
        Func<ISettingEditorContext, TAccept, IDisplayable<string>> Validator { get; set; }
        Expression<Func<TSetting, TStore>> Property { get; }
        ISettingEditor Parent { get; }

        IValueEditorBuilder<TSetting, TStore, TAccept> SetName(string name);
        IValueEditorBuilder<TSetting, TStore, TAccept> SetAlias(params string[] aliases);
        IValueEditorBuilder<TSetting, TStore, TAccept> SetViewer(Func<ISettingEditorContext, TStore, IDisplayable<string>> viewer);
        IValueEditorBuilder<TSetting, TStore, TAccept> SetValidator(Func<ISettingEditorContext, TAccept, IDisplayable<string>> validator);
        IValueEditorBuilder<TSetting, TStore, TAccept> AllowGroups(bool allow = true);

        IValueEditor Build();
    }
}