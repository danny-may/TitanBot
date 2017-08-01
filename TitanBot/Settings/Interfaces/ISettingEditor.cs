using Discord;
using System;
using TitanBot.Contexts;

namespace TitanBot.Settings
{
    public interface ISettingEditor
    {
        string Name { get; }
        string[] Aliases { get; }
        Type Type { get; }
        bool TrySet(ICommandContext context, IEntity<ulong> entity, object value, out string errors);
        string Display(ICommandContext context, IEntity<ulong> entity);
        object Get(IEntity<ulong> entity);
    }
}
