using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Titansmasher.Extensions;

namespace Titanbot.Core.Command.Models
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
        public string SubCall { get; }
        public string[] Aliases { get; }
        public bool Hidden { get; }
        public FlagRequirement Flags { get; }
        public IReadOnlyList<ArgumentInfo> Parameters { get; }
        public IReadOnlyList<IReadOnlyList<ArgumentInfo>> ParameterPermatations { get; }

        #endregion Fields

        #region Constructors

        private CallInfo(MethodInfo method, CommandInfo parent)
        {
            Method = method ?? throw new System.ArgumentNullException(nameof(method));
            Parent = parent ?? throw new System.ArgumentNullException(nameof(parent));
            Usage = UsageAttribute.GetFor(Method);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(Method);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(Method);
            RequiredContexts = RequireContextAttribute.GetFor(Method);
            RequireOwner = RequireOwnerAttribute.ExistsOn(Method);
            ShowTyping = !HideTypingAttribute.ExistsOn(Method);
            RequireReply = !NoReplyAttribute.ExistsOn(Method);
            SubCall = CallAttribute.GetFor(Method);
            Aliases = AliasAttribute.GetFor(Method);
            Hidden = HiddenAttribute.ExistsOn(Method);
            Flags = new FlagRequirement(this);
            Parameters = ArgumentInfo.BuildFrom(this);
            ParameterPermatations = GetPermatations();
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
    }
}