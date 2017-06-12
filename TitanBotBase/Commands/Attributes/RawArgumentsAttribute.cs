using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RawArgumentsAttribute : Attribute
    {
        public static bool ExistsOn(ParameterInfo info)
            => info.GetCustomAttribute<RawArgumentsAttribute>() != null;
    }
}
