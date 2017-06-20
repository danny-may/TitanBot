using System;
using System.Reflection;

namespace TitanBot.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DenseAttribute : Attribute
    {
        public static bool ExistsOn(ParameterInfo info)
            => info.GetCustomAttribute<DenseAttribute>() != null;
    }
}
