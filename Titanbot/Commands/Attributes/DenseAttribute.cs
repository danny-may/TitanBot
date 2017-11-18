using System;
using System.Reflection;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class DenseAttribute : Attribute
    {
        #region Statics

        public static bool ExistsOn(ParameterInfo parameter)
            => parameter.GetCustomAttribute<DenseAttribute>() != null;

        #endregion Statics
    }
}