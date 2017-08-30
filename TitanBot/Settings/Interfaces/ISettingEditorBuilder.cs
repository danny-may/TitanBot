using System;
using TitanBot.Contexts;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Settings
{
    public interface ISettingEditorBuilder<TStore, TAccept>
    {
        string Name { get; }
        bool Groups { get; }
        string[] Aliases { get; }
        Func<IMessageContext, TStore, ILocalisable<string>> Viewer { get; }
        Func<IMessageContext, TAccept, TStore> Converter { get; }
        Func<IMessageContext, TAccept, ILocalisable<string>> Validator { get; }

        ISettingEditorBuilder<TStore, TAccept> SetName(string name);

        ISettingEditorBuilder<TStore, TAccept> SetAlias(params string[] aliases);

        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<IMessageContext, TStore, ILocalisable<string>> viewer);

        ISettingEditorBuilder<TStore, TAccept> SetViewer(Func<TStore, ILocalisable<string>> viewer);

        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<IMessageContext, TAccept, ILocalisable<string>> validator);

        ISettingEditorBuilder<TStore, TAccept> SetValidator(Func<TAccept, ILocalisable<string>> validator);

        ISettingEditorBuilder<TStore, TAccept> AllowGroups(bool allow = true);

        ISettingEditor Build();
    }
}