using System;
using System.Reflection;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Services.CommandService.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    class DenseAttribute : Attribute
    {
        public static bool GetFrom(ParameterInfo info)
            => info.GetCustomAttribute<DenseAttribute>() != null;
        public static bool GetFrom(ArgumentInfo info)
            => GetFrom(info.Parameter);
    }
}
