using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.TypeReaders;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Responses;

namespace TitanBot2.Services.CommandService.Models
{
    public class CommandInfo
    {
        public Type CommandType { get; }
        public string Group => GroupAttribute.GetFrom(this);
        public string Name => NameAttribute.GetFrom(this);
        public string[] Alias => AliasAttribute.GetFom(this);
        public string Description => DescriptionAttribute.GetFrom(this);
        public ulong DefaultPermissions => DefaultPermissionAttribute.GetFrom(this);
        public ContextType RequiredContexts => RequireContextAttribute.GetFrom(this);
        public string PermissionKey => DefaultPermissionAttribute.GetKey(this);
        public string Note => NotesAttribute.GetFrom(this);
        public bool RequireOwner => RequireOwnerAttribute.GetFrom(this);
        public ulong? RequireGuild => RequireGuildAttribute.GetFrom(this);

        public CallInfo[] Calls { get; }

        private CommandInfo(Type commandType)
        {
            CommandType = commandType;
            Calls = CallInfo.FromCommandInfo(this);
        }

        public Command CreateInstance(CmdContext context, TypeReaderCollection readers)
        {
            var res = Activator.CreateInstance(CommandType) as Command;
            res.SetContext(context, readers);
            return res;
        }

        public async Task<Dictionary<CallInfo, CallCheckResponse>> CheckCalls(CmdContext context, TypeReaderCollection readers)
        {
            if (Calls.Length == 0)
                return new Dictionary<CallInfo, CallCheckResponse>();
            var inst = CreateInstance(context, readers);
            var checking = Calls.ToDictionary(c => c, c => inst.CheckCallAsync(c));
            await Task.WhenAll(checking.Select(c => c.Value));
            return checking.ToDictionary(c => c.Key, c => c.Value.Result);
        }

        public static CommandInfo FromType(Type t)
        {
            if (!typeof(Command).IsAssignableFrom(t))
                throw new InvalidOperationException("Cannot create CommandInfo from type " + t.Name);
            return new CommandInfo(t);
        }
    }
}
