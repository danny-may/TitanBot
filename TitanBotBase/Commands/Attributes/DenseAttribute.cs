using System;
using System.Reflection;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DenseAttribute : Attribute
    {
        public static bool ExistsOn(ParameterInfo info)
            => info.GetCustomAttribute<DenseAttribute>() != null;
    }
}
