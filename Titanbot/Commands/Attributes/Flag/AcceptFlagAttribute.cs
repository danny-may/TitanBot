using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Titanbot.Commands.Models;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AcceptFlagAttribute : Attribute
    {
        #region Statics

        public static bool ExistsOn(MethodInfo method)
            => method.GetCustomAttribute<AcceptFlagAttribute>() != null;

        public static IReadOnlyList<FlagInfo> GetFor(MethodInfo method, IReadOnlyList<FlagInfo> flags)
            => method.GetCustomAttribute<AcceptFlagAttribute>()?.GetAcceptedFlags(flags)
                                                                .Concat(RequireFlagAttribute.GetFor(method, flags))
                                                                .ToList()
                                                                .AsReadOnly() ?? new List<FlagInfo>().AsReadOnly();

        public static bool IsStrict(MethodInfo method)
            => method.GetCustomAttribute<AcceptFlagAttribute>()?.Strict ?? false;

        #endregion Statics

        #region Fields

        private string[] _properties { get; } = new string[0];
        private char[] _ids { get; } = new char[0];
        public bool Strict { get; }

        #endregion Fields

        #region Constructors

        public AcceptFlagAttribute(params string[] propertyNames)
            => _properties = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));

        public AcceptFlagAttribute(params char[] ids)
            => _ids = ids ?? throw new ArgumentNullException(nameof(ids));

        #endregion Constructors

        #region Methods

        public IReadOnlyList<FlagInfo> GetAcceptedFlags(IReadOnlyList<FlagInfo> flags)
            => flags.Where(f => _ids.Contains(f.Id) ||
                                _properties.Contains(f.Property.Name))
                    .ToList()
                    .AsReadOnly();

        #endregion Methods
    }
}