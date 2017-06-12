using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Util;

namespace TitanBotBase.Commands
{
    public struct ArgumentInfo
    {
        public ParameterInfo Parameter { get; }
        public Type Type { get; }
        public object DefaultValue { get; }
        public bool IsDense { get; }
        public bool IsRawArgument { get; }
        public bool HasDefaultValue { get; }
        public string Name { get; }
        public bool UseDefault { get; }

        private ArgumentInfo(ParameterInfo parameter, bool useDefault)
        {
            Parameter = parameter;
            UseDefault = useDefault;
            IsDense = DenseAttribute.ExistsOn(parameter);
            Type = Parameter.ParameterType;
            DefaultValue = Parameter.DefaultValue;
            IsRawArgument = RawArgumentsAttribute.ExistsOn(parameter);
            HasDefaultValue = Parameter.HasDefaultValue;
            Name = NameAttribute.GetFor(Parameter);
        }

        internal static IEnumerable<ArgumentInfo[]> BuildPermetations(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var required = parameters.Where(p => !p.IsOptional);
            var optional = parameters.Where(p => p.IsOptional);

            var requiredMask = Enumerable.Range(0, required.Count())
                                         .Select(v => true)
                                         .ToArray();
            foreach (var optionalMask in IEnumerableUtil.BinomialMask(optional.Count()))
            {
                var fullMask = requiredMask.Concat(optionalMask).ToArray();
                var args = parameters.Select((p, i) => new ArgumentInfo(p, !fullMask[i])).ToArray();
                if (args.Count(a => a.IsDense) <= 1)
                    yield return args;
            }
        }

        internal static ArgumentInfo[] BuildFrom(MethodInfo call)
            => call.GetParameters().Select(p => new ArgumentInfo(p, false)).ToArray();
    }
}
