using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Util;

namespace TitanBotBase.Commands
{
    public struct CallInfo
    {
        public IReadOnlyList<ArgumentInfo[]> ArgumentPermatations { get; }
        public MethodInfo Call { get; }
        public string Usage { get; }
        public ulong DefaultPermissions { get; }
        public string PermissionKey { get; }
        public ContextType RequiredContexts { get; }
        public bool RequireOwner { get; }
        //public FlagInfo[] Flags { get; }
        public string SubCall { get; }
        public ArgumentInfo[] Parameters { get; }
        public FlagDefinition[] Flags { get; }

        private CallInfo(MethodInfo method)
        {
            Call = method;
            if (!CallAttribute.ExistsOn(Call))
                throw new InvalidOperationException($"Cannot create CallInfo from {Call} as it does not have the [Call] attribute");
            Usage = UsageAttribute.GetFor(Call);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(Call);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(Call);
            RequiredContexts = RequireContextAttribute.GetFor(Call);
            RequireOwner = RequireOwnerAttribute.ExistsOn(Call);
            SubCall = CallAttribute.GetFor(Call);
            var args = ArgumentInfo.BuildFrom(Call);
            if (args.SkipWhile(p => p.Flag == null).Select(p => p.Flag).Contains(null))
                throw new InvalidOperationException("All optional arguments after the first flag must also be flags");
            Parameters = args.TakeWhile(a => a.Flag == null).ToArray();
            Flags = args.SkipWhile(a => a.Flag == null).Select(a => a.Flag).ToArray();
            ArgumentPermatations = ArgumentInfo.BuildPermetations(Call).ToList().AsReadOnly();
        }

        internal static IEnumerable<CallInfo> BuildFrom(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.ReturnType == typeof(Task) && CallAttribute.ExistsOn(m)))
            {
                var built = new CallInfo(method);
                if (built.ArgumentPermatations.Count == 0)
                    continue;
                yield return built;
            }
        }

        public string[] GetParameters()
            => Parameters.Select(p => p.ToString()).ToArray();

        public string GetFlags()
            => ("-" + string.Join("", Flags.Select(f => f.ShortKey))).TrimEnd('-');
    }
}
