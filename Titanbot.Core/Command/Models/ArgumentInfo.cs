using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Titanbot.Core.Command.Models
{
    public class ArgumentInfo //: ILocalisable<string>
    {
        #region Statics

        public static IReadOnlyList<ArgumentInfo> BuildFrom(CallInfo parent)
            => parent.Method.GetParameters()
                            .Select(p => new ArgumentInfo(p, parent))
                            .ToList()
                            .AsReadOnly();

        #endregion Statics

        #region Fields

        public ParameterInfo Parameter { get; }
        public Type Type => Parameter.ParameterType;
        public object DefaultValue => Parameter.DefaultValue;
        public bool IsDense { get; }
        public bool HasDefaultValue => Parameter.HasDefaultValue;
        public string Name { get; }
        public CallInfo Parent { get; }

        #endregion Fields

        #region Constructors

        private ArgumentInfo(ParameterInfo parameter, CallInfo parent)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            IsDense = DenseAttribute.ExistsOn(Parameter);
            Name = NameAttribute.GetFor(Parameter);
        }

        #endregion Constructors
    }
}