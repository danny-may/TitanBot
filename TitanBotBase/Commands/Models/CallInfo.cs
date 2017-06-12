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
            Parameters = ArgumentInfo.BuildFrom(Call).ToArray();
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

        public string[] GetParameters(string requiredFormat = "<{0}>", string optionalFormat = "[{0}]", string arrayFormat = "{0}...")
        {
            var pars = Call.GetParameters();

            var ret = new List<string>();

            foreach (var param in pars)
            {
                var paramFormat = param.IsOptional ? optionalFormat : requiredFormat;
                paramFormat = string.Format(paramFormat, typeof(IEnumerable<>).IsAssignableFrom(param.ParameterType) || param.ParameterType.IsArray ? arrayFormat : "{0}");
                ret.Add(string.Format(paramFormat, NameAttribute.GetFor(param)));
            }

            return ret.ToArray();
        }
    }
}
