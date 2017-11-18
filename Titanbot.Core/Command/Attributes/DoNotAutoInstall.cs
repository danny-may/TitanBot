using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DoNotAutoInstall : Attribute
    {
        public static bool ExistsOn(Type info)
            => info.GetCustomAttribute<DoNotAutoInstall>() != null;
    }
}