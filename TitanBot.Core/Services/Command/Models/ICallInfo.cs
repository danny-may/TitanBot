using System.Collections.Generic;
using System.Reflection;

namespace TitanBot.Core.Services.Command.Models
{
    public interface ICallInfo
    {
        string[] Aliases { get; }
        IReadOnlyList<IArgumentCollection> ArgumentPermatations { get; }
        ulong DefaultPermissions { get; }
        bool Hidden { get; }
        MethodInfo Method { get; }
        IArgumentCollection Parameters { get; }
        ICommandInfo Parent { get; }
        string PermissionKey { get; }
        ContextType RequiredContexts { get; }
        bool RequireOwner { get; }
        bool ShowTyping { get; }
        string SubCall { get; }
        string Usage { get; }

        string[] GetParameters();
    }
}