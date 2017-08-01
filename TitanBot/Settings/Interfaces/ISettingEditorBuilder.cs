using System;
using TitanBot.Contexts;

namespace TitanBot.Settings
{
    public interface ISettingEditorBuilder<TStore, TAccept>
    {
        string Name { get; }
        string[] Aliases { get; }
        Func<IMessageContext, TStore, string> Viewer { get; }
        Func<IMessageContext, TAccept, TStore> Converter { get; }
        Func<IMessageContext, TAccept, string> Validator { get; }

        ISettingEditorBuilder<TStore, TAccept> SetName(string name);
        ISettingEditorBuilder<TStore, TAccept> SetAlias(params string[] aliases);

        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<IMessageContext, TStore, string> viewer);
        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, string> viewer);

        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<IMessageContext, TAccept, string> validator);
        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, string> validator);

        ISettingEditor Build();
    }
}
