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
        public ulong DefaultPermissions => DefaultPermissionAttribute.GetPerm(this);
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

        public Command CreateInstance(CmdContext context)
        {
            var res = Activator.CreateInstance(CommandType) as Command;
            res.SetContext(context);
            return res;
        }

        public Task<Dictionary<CallInfo, CallCheckResponse>> CheckCalls(CmdContext context)
        {
            if (Calls.Length == 0)
                return Task.FromResult(new Dictionary<CallInfo, CallCheckResponse>());
            var inst = CreateInstance(context);
            return inst.CheckCallsAsync(Calls);
        }

        public static CommandInfo FromType(Type t)
        {
            if (!typeof(Command).IsAssignableFrom(t))
                throw new InvalidOperationException("Cannot create CommandInfo from type " + t.Name);
            return new CommandInfo(t);
        }
    }
}
