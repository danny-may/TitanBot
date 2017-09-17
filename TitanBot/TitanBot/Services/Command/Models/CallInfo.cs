using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    public class CallInfo : ICallInfo
    {
        #region Statics

        internal static ICallCollection BuildFrom(ICommandInfo info)
        {
            IEnumerable<MethodInfo> methods = info.CommandType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            methods = methods.Where(m => m.ReturnType == typeof(Task) && CallAttribute.ExistsOn(m) && !DoNotInstallAttribute.ExistsOn(m));

            var built = methods.Select(m => new CallInfo(m, info))
                               .Where(m => m.ArgumentPermatations.Count > 0);

            return new CallCollection(built);
        }

        #endregion Statics

        #region Constructors

        private CallInfo(MethodInfo method, ICommandInfo info)
        {
            Method = method;
            if (!CallAttribute.ExistsOn(Method))
                throw new InvalidOperationException($"Cannot create CallInfo from {Method} as it does not have the [Call] attribute");
            Usage = UsageAttribute.GetFor(Method);
            DefaultPermissions = DefaultPermissionAttribute.GetPermFor(Method);
            PermissionKey = DefaultPermissionAttribute.GetKeyFor(Method).ToLower();
            RequiredContexts = RequireContextAttribute.GetFor(Method);
            RequireOwner = RequireOwnerAttribute.ExistsOn(Method);
            ShowTyping = !HideTypingAttribute.ExistsOn(Method);
            Aliases = AliasAttribute.GetFor(Method);
            Hidden = HiddenAttribute.ExistsOn(Method);
            Parent = info;
            Parameters = null;
            ArgumentPermatations = null;

            Parameters = ArgumentInfo.BuildFrom(this);
            ArgumentPermatations = BuildArgPermatations();
        }

        #endregion Constructors

        #region Methods

        private IReadOnlyList<IArgumentCollection> BuildArgPermatations()
        {
            List<IArgumentCollection> permatations = new List<IArgumentCollection>();

            var required = Parameters.Where(p => !p.IsOptional).ToArray();
            var optional = Parameters.Where(p => p.IsOptional).ToArray();

            var requiredMask = Enumerable.Range(0, required.Count())
                                         .Select(v => true)
                                         .ToArray();

            var counter = (int)Math.Pow(2, optional.Length);
            while (counter-- > 0)
            {
                var mask = requiredMask.Concat(new BitArray(new int[] { counter }).Cast<bool>().Take(optional.Length).Reverse().ToArray());
                var collection = new ArgumentCollection(Parameters.Zip(mask, (p, b) => b ? p : null));
                if (collection.DenseCount <= 1)
                    permatations.Add(collection);
            }

            return permatations.Where(p => p.Count(a => a?.IsDense ?? false) <= 1).ToImmutableList();
        }

        #endregion Methods

        #region Overrides

        public override string ToString()
            => PermissionKey;

        #endregion Overrides

        #region ICallInfo

        public IReadOnlyList<IArgumentCollection> ArgumentPermatations { get; }
        public MethodInfo Method { get; }
        public string Usage { get; }
        public ulong DefaultPermissions { get; }
        public string PermissionKey { get; }
        public ContextType RequiredContexts { get; }
        public bool RequireOwner { get; }
        public bool ShowTyping { get; }
        public string SubCall { get; }
        public string[] Aliases { get; }
        public IArgumentCollection Parameters { get; }
        public ICommandInfo Parent { get; }
        public bool Hidden { get; }

        public string[] GetParameters()
            => Parameters.Select(p => p.ToString()).ToArray();

        #endregion ICallInfo
    }
}