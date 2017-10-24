using System;

namespace TitanBot.Core.Services.Command.Models
{
    public interface ICommandInfo
    {
        string[] Alias { get; }
        ICallCollection Calls { get; }
        Type CommandType { get; }
        ulong DefaultPermissions { get; }
        string Description { get; }
        string Group { get; }
        bool Hidden { get; }
        string Name { get; }
        string Note { get; }
        string PermissionKey { get; }
        ContextType RequiredContexts { get; }
        ulong[] RequireGuild { get; }
        bool RequireOwner { get; }
    }
}