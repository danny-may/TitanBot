using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public struct CallInfo
    {
        public IReadOnlyList<ArgumentInfo[]> ArgumentPermatations { get; }
        public MethodInfo Method { get; }
        public string Usage { get; }
        public ulong DefaultPermissions { get; }
        public string PermissionKey { get; }
        public ContextType RequiredContexts { get; }
        public bool RequireOwner { get; }
        public bool ShowTyping { get; }
        public string SubCall { get; }
        public string[] Aliases { get; }
        public ArgumentInfo[] Parameters { get; }
        public CommandInfo Parent { get; }
        public FlagDefinition[] Flags { get; }
        public bool Hidden { get; }

        private CallInfo(MethodInfo method, CommandInfo info)
        {
            Method = method;
            if (!CallAttribute.ExistsOn(Method))
                throw new InvalidOperationException($"Cannot create CallInfo from {Method} as it does not have the [Call] attribute");
            Usage = UsageAttribute.GetFor(Method);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(Method);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(Method);
            RequiredContexts = RequireContextAttribute.GetFor(Method);
            RequireOwner = RequireOwnerAttribute.ExistsOn(Method);
            ShowTyping = !HideTypingAttribute.ExistsOn(Method);
            SubCall = CallAttribute.GetFor(Method);
            Aliases = AliasAttribute.GetFor(Method);
            Hidden = HiddenAttribute.ExistsOn(Method);
            Parent = info;
            Parameters = null;
            Flags = null;
            ArgumentPermatations = null;

            var args = ArgumentInfo.BuildFrom(this);
            if (args.SkipWhile(p => p.Flag == null).Select(p => p.Flag).Contains(null))
                throw new InvalidOperationException("All optional arguments after the first flag must also be flags");
            Parameters = args.TakeWhile(a => a.Flag == null).ToArray();
            Flags = args.SkipWhile(a => a.Flag == null).Select(a => a.Flag).ToArray();
            ArgumentPermatations = ArgumentInfo.BuildPermetations(this).ToList().AsReadOnly();
        }

        internal static IEnumerable<CallInfo> BuildFrom(CommandInfo info)
        {
            foreach (var method in info.CommandType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.ReturnType == typeof(Task) && CallAttribute.ExistsOn(m) && !DoNotInstallAttribute.ExistsOn(m)))
            {
                var built = new CallInfo(method, info);
                if (built.ArgumentPermatations.Count == 0)
                    continue;
                yield return built;
            }
        }

        public string[] GetParameters()
            => Parameters.Select(p => p.ToString()).ToArray();

        public string GetFlags()
            => ("-" + string.Join("", Flags.Select(f => f.ShortKey))).TrimEnd('-');

        public override string ToString()
            => PermissionKey;
    }
}
