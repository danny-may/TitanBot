using System;
using System.Linq;
using System.Reflection;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    public class ArgumentInfo : IArgumentInfo
    {
        #region Statics

        internal static IArgumentCollection BuildFrom(CallInfo call)
            => new ArgumentCollection(call.Method.GetParameters().Select(p => new ArgumentInfo(p, call)));

        #endregion Statics

        #region Constructors

        private ArgumentInfo(ParameterInfo param, ICallInfo parent)
        {
            Parameter = param;
            IsDense = DenseAttribute.ExistsOn(param);
            IsRaw = RawArgumentsAttribute.ExistsOn(param);
            IsLiteral = LiteralAttribute.ExistsOn(param);
            Literals = LiteralAttribute.GetFor(param);
            Name = NameAttribute.GetFor(param);
            Parent = parent;
            if (IsDense && IsLiteral)
                throw new InvalidOperationException("Cannot have a dense literal");
        }

        #endregion Constructors

        #region IArgumentInfo

        public ParameterInfo Parameter { get; }
        public Type Type => Parameter.ParameterType;
        public object DefaultValue => Parameter.DefaultValue;
        public bool IsOptional => Parameter.HasDefaultValue;
        public bool IsDense { get; }
        public bool IsRaw { get; }
        public bool IsLiteral { get; }
        public object[] Literals { get; }
        public string Name { get; }
        public ICallInfo Parent { get; }

        #endregion IArgumentInfo
    }
}