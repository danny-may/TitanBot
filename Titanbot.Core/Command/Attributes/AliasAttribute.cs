using System;
using System.Reflection;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AliasAttribute : Attribute
    {
        #region Statics

        public static string[] GetFor(MethodInfo method)
            => method.GetCustomAttribute<AliasAttribute>()?.Aliases ?? new string[0];
        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<AliasAttribute>() != null;

        public static string[] GetFor(Type type)
            => type.GetCustomAttribute<AliasAttribute>()?.Aliases ?? new string[0];
        public static bool ExistsOn(Type type)
            => type.GetCustomAttribute<AliasAttribute>() != null;

        #endregion Statics

        #region Fields

        public string[] Aliases { get; }

        #endregion Fields

        #region Constructors

        public AliasAttribute(string[] aliases)
            => Aliases = aliases;

        #endregion Constructors
    }
}