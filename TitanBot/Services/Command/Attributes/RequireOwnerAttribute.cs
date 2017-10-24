using System;
using System.Reflection;

namespace TitanBot.Services.Command
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireOwnerAttribute : Attribute
    {
        public static bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<RequireOwnerAttribute>() != null || ExistsOn(info.DeclaringType);
        
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<RequireOwnerAttribute>() != null;
    }
}
