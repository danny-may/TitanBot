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
        bool AllowGroups { get; }
        Type Type { get; }
        bool TrySet(ICommandContext context, IEntity<ulong> entity, int group, object value, out ILocalisable<string> errors);
        ILocalisable<string> Display(ICommandContext context, IEntity<ulong> entity, int group);
        object Get(IEntity<ulong> entity, int group);
    }
}
