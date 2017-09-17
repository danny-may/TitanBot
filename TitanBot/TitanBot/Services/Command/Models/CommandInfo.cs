using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    public class CommandInfo : ICommandInfo
    {
        #region Statics

        public static CommandInfo[] Build(IEnumerable<Type> types)
            => types.Where(t => t.GetInterface<ICommand>() != null)
                    .Select(t => new CommandInfo(t))
                    .Where(c => c.Calls.Count > 0)
                    .ToArray();

        #endregion Statics

        #region Constructors

        private CommandInfo(Type type)
        {
            if (type.GetInterface<ICommand>() == null)
                throw new InvalidOperationException($"Cannot create CommandInfo from {type}");
            CommandType = type;
            Group = GroupAttribute.GetFor(type);
            Name = NameAttribute.GetFor(type);
            Alias = AliasAttribute.GetFor(type);
            Description = DescriptionAttribute.GetFor(type);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(type);
            RequiredContexts = RequireContextAttribute.GetFor(type);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(type);
            Note = NotesAttribute.GetFor(type);
            RequireOwner = RequireOwnerAttribute.ExistsOn(type);
            RequireGuild = RequireGuildAttribute.GetFor(type);
            Hidden = false;
            Calls = null;
            Calls = CallInfo.BuildFrom(this);
            Hidden = HiddenAttribute.ExistsOn(type) || Calls.All(c => c.Hidden);
        }

        #endregion Constructors

        #region Overrides

        public override string ToString()
            => Name;

        #endregion Overrides

        #region ICommandInfo

        public ICallCollection Calls { get; }
        public Type CommandType { get; }
        public string Group { get; }
        public string Name { get; }
        public string[] Alias { get; }
        public string Description { get; }
        public ulong DefaultPermissions { get; }
        public ContextType RequiredContexts { get; }
        public string PermissionKey { get; }
        public string Note { get; }
        public bool RequireOwner { get; }
        public ulong[] RequireGuild { get; }
        public bool Hidden { get; }

        #endregion ICommandInfo
    }
}