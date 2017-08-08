using Discord;
using System;
using TitanBot.Contexts;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Settings
{
    public interface ISettingEditor
    {
        string Name { get; }
        string[] Aliases { get; }
        Type Type { get; }
        bool TrySet(ICommandContext context, IEntity<ulong> entity, object value, out ILocalisable<string> errors);
        ILocalisable<string> Display(ICommandContext context, IEntity<ulong> entity);
        object Get(IEntity<ulong> entity);
    }
}
