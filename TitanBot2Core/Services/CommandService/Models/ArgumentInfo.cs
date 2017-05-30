using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.TypeReaders;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Models
{
    public class ArgumentInfo
    {
        public bool IsDense => DenseAttribute.GetFrom(this);
        public Type ArgType => Parameter.ParameterType;
        public string Name => NameAttribute.GetFrom(this);
        public bool Optional => Parameter.HasDefaultValue;
        public object DefaultValue => Parameter.DefaultValue;
        public ParameterInfo Parameter { get; }
        public CallInfo ParentInfo { get; }

        public ArgumentInfo(ParameterInfo info, CallInfo parent)
        {
            Parameter = info;
            ParentInfo = parent;
        }

        public static ArgumentInfo[] FromCallInfo(CallInfo info)
            => info.Call.GetParameters().Select(p => new ArgumentInfo(p, info)).ToArray();
    }
}
