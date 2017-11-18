using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Titanbot.Commands.Models;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireFlagAttribute : Attribute
    {
        #region Statics

        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<RequireFlagAttribute>() != null;

        public static IReadOnlyList<FlagInfo> GetFor(MethodInfo method, IReadOnlyList<FlagInfo> flags)
            => method.GetCustomAttribute<RequireFlagAttribute>()?.GetRequiredFlags(flags) ?? new FlagInfo[0];

        #endregion Statics

        #region Fields

        private string[] _properties { get; } = new string[0];
        private char[] _ids { get; } = new char[0];

        #endregion Fields

        #region Constructors

        public RequireFlagAttribute(params string[] propertyNames)
            => _properties = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));

        public RequireFlagAttribute(params char[] ids)
            => _ids = ids ?? throw new ArgumentNullException(nameof(ids));

        #endregion Constructors

        #region Methods

        public IReadOnlyList<FlagInfo> GetRequiredFlags(IReadOnlyList<FlagInfo> flags)
            => flags.Where(f => _ids.Contains(f.Id) ||
                                _properties.Contains(f.Property.Name))
                    .ToList()
                    .AsReadOnly();

        #endregion Methods
    }
}