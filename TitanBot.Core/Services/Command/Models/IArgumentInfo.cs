using System;
using System.Reflection;

namespace TitanBot.Core.Services.Command.Models
{
    public interface IArgumentInfo
    {
        object DefaultValue { get; }
        bool IsDense { get; }
        bool IsLiteral { get; }
        bool IsOptional { get; }
        bool IsRaw { get; }
        object[] Literals { get; }
        string Name { get; }
        ParameterInfo Parameter { get; }
        ICallInfo Parent { get; }
        Type Type { get; }
    }
}