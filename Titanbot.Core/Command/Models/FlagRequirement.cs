using System.Collections.Generic;

namespace Titanbot.Command.Models
{
    public class FlagRequirement
    {
        #region Fields

        public IReadOnlyList<FlagInfo> Required { get; }
        public IReadOnlyList<FlagInfo> Accepted { get; }
        public bool Strict { get; }

        #endregion Fields

        #region Constructors

        public FlagRequirement(CallInfo call)
        {
            Required = RequireFlagAttribute.GetFor(call.Method, call.Parent.Flags);
            Accepted = AcceptFlagAttribute.GetFor(call.Method, call.Parent.Flags);
            Strict = AcceptFlagAttribute.IsStrict(call.Method);
        }

        #endregion Constructors
    }
}