using System;
using TitanBot.Commands;

namespace TitanBot.Settings
{
    public interface ISettingEditorBuilder<TStore, TAccept>
    {
        string Name { get; }
        string[] Aliases { get; }
        Func<ICommandContext, TStore, string> Viewer { get; }
        Func<ICommandContext, TAccept, TStore> Converter { get; }
        Func<ICommandContext, TAccept, string> Validator { get; }

        ISettingEditorBuilder<TStore, TAccept> SetName(string name);
        ISettingEditorBuilder<TStore, TAccept> SetAlias(params string[] aliases);

        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<ICommandContext, TStore, string> viewer);
        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, string> viewer);

        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<ICommandContext, TAccept, string> validator);
        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, string> validator);

        ISettingEditor Build();
    }
}
