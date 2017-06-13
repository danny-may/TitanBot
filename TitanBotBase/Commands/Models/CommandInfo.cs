using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands
{
    public struct CommandInfo
    {
        public IReadOnlyList<CallInfo> Calls { get; }
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
        public ulong? RequireGuild { get; }

        private CommandInfo(Type type)
        {
            if (!type.IsSubclassOf(typeof(Command)))
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
            Calls = null;
            Calls = CallInfo.BuildFrom(this).ToList().AsReadOnly();
        }

        internal static IEnumerable<CommandInfo> BuildFrom(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var built = new CommandInfo(type);
                if (built.Calls.Count == 0)
                    continue;
                yield return built;
            }
        }

        public override string ToString()
            => Name;
    }
}
