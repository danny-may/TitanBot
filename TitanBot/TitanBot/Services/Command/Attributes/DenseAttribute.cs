using System;
using System.Reflection;

namespace TitanBot.Services.Command
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DenseAttribute : Attribute
    {
        public static bool ExistsOn(ParameterInfo info)
            => info.GetCustomAttribute<DenseAttribute>() != null;
    }
}
