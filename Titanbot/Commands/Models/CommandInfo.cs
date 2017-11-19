using System;
using System.Collections.Generic;
using System.Linq;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands.Models
{
    public class CommandInfo
    {
        #region Statics

        public static IReadOnlyList<CommandInfo> BuildFor(IEnumerable<Type> types)
            => types?.Where(t => t.IsSubclassOf(typeof(CommandBase)) &&
                                 !t.IsAbstract)
                     .Select(t => new CommandInfo(t))
                     .Where(c => c.Calls.Count > 0)
                     .ToList()
                     .AsReadOnly() ?? new List<CommandInfo>().AsReadOnly();

        #endregion Statics

        #region Fields

        public Type CommandType { get; }
        public string Group { get; }
        public string Name { get; }
        public string[] Alias { get; }
        public IDisplayable<string> Description { get; }
        public ulong DefaultPermissions { get; }
        public string PermissionKey { get; }
        public bool RequireReply { get; }
        public ContextType RequiredContexts { get; }
        public IDisplayable<string> Note { get; }
        public bool RequireOwner { get; }
        public ulong[] RequireGuild { get; }
        public bool Hidden { get; }
        public IReadOnlyList<FlagInfo> Flags { get; }
        public IReadOnlyList<CallInfo> Calls { get; }

        #endregion Fields

        #region Constructors

        private CommandInfo(Type type)
        {
            CommandType = type ?? throw new ArgumentNullException(nameof(type));
            Group = GroupAttribute.GetFor(CommandType);
            Name = NameAttribute.GetFor(CommandType);
            Alias = AliasAttribute.GetFor(CommandType);
            Description = DescriptionAttribute.GetFor(CommandType);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(CommandType);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(CommandType);
            RequireReply = !NoReplyAttribute.ExistsOn(CommandType);
            RequiredContexts = RequireContextAttribute.GetFor(CommandType);
            Note = IncludeNotesAttribute.GetFor(CommandType);
            RequireOwner = RequireOwnerAttribute.ExistsOn(CommandType);
            RequireGuild = RequireGuildAttribute.GetFor(CommandType);
            Hidden = HiddenAttribute.ExistsOn(CommandType);
            Flags = FlagInfo.BuildFrom(this);
            Calls = CallInfo.BuildFrom(this);
        }

        #endregion Constructors

        #region Overrides

        public override string ToString()
            => $"{Group}/{Name}";

        #endregion Overrides
    }
}