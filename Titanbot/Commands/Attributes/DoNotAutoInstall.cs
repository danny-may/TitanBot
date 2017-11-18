using System;
using System.Reflection;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DoNotAutoInstall : Attribute
    {
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<DoNotAutoInstall>() != null;
    }
}