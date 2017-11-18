using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Titansmasher.Extensions;

namespace Titanbot.Commands.Models
{
    public class CallInfo //: ILocalisable<string>
    {
        #region Statics

        public static IReadOnlyList<CallInfo> BuildFrom(CommandInfo parent)
            => parent.CommandType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(m => (m.ReturnType == typeof(Task) ||
                                                (m.ReturnType.IsGenericType &&
                                                    m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))) &&
                                             CallAttribute.ExistsOn(m))
                                 .Select(m => new CallInfo(m, parent))
                                 .ToList()
                                 .AsReadOnly();

        #endregion Statics

        #region Fields

        public MethodInfo Method { get; }
        public CommandInfo Parent { get; }
        //public ILocalisable<string> Usage { get; }
        public string Usage { get; }
        public ulong DefaultPermissions { get; }
        public string PermissionKey { get; }
        public ContextType RequiredContexts { get; }
        public bool RequireOwner { get; }
        public bool ShowTyping { get; }
        public bool RequireReply { get; }
        public string[] ArgumentMask { get; }
        public string[] Aliases { get; }
        public bool Hidden { get; }
        public FlagRequirement Flags { get; }
        public IReadOnlyList<ArgumentInfo> Parameters { get; }
        public IReadOnlyList<IReadOnlyList<ArgumentInfo>> ParameterPermatations { get; }

        #endregion Fields

        #region Constructors

        private CallInfo(MethodInfo method, CommandInfo parent)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Usage = UsageAttribute.GetFor(Method);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(Method);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(Method);
            RequiredContexts = RequireContextAttribute.GetFor(Method);
            RequireOwner = RequireOwnerAttribute.ExistsOn(Method);
            ShowTyping = !HideTypingAttribute.ExistsOn(Method);
            RequireReply = !NoReplyAttribute.ExistsOn(Method);
            ArgumentMask = CallAttribute.GetFor(Method);
            Aliases = AliasAttribute.GetFor(Method);
            Hidden = HiddenAttribute.ExistsOn(Method);
            Flags = new FlagRequirement(this);
            Parameters = ArgumentInfo.BuildFrom(this);
            ParameterPermatations = GetPermatations();

            var maskArgCount = ArgumentMask.Count(a => a == null);
            if (maskArgCount > Parameters.Count)
                throw new ArgumentException($"Argument mask expecting more arguments than method accepts. Max {Parameters.Count}, got {maskArgCount}");
        }

        #endregion Constructors

        #region Methods

        private IReadOnlyList<IReadOnlyList<ArgumentInfo>> GetPermatations()
        {
            var required = Parameters.Where(p => !p.HasDefaultValue).ToList();
            var optional = Parameters.Where(p => p.HasDefaultValue).ToList();

            return optional.GetPermatations()
                           .Select(p => required.Concat(p)
                                                .ToList()
                                                .AsReadOnly())
                           .ToList()
                           .AsReadOnly();
        }

        #endregion Methods

        #region Overrides

        public override string ToString()
            => string.Join("/", ArgumentMask.Where(a => a != null));

        #endregion Overrides
    }
}