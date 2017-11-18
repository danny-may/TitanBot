using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Titanbot.Command.Models
{
    public class FlagInfo //: ILocalisable<string>
    {
        #region Statics

        public static IReadOnlyList<FlagInfo> BuildFrom(CommandInfo parent)
            => parent.CommandType.GetProperties()
                                 .Where(p => CommandFlagAttribute.ExistsOn(p))
                                 .Select(p => new FlagInfo(p, parent))
                                 .ToList()
                                 .AsReadOnly();

        #endregion Statics

        #region Fields

        public PropertyInfo Property { get; }
        private CommandFlagAttribute _cmdAttr { get; }
        public char Id => _cmdAttr.Id;
        public string Name => _cmdAttr.Name;
        //public ILocalisable<string> Description => _cmdAttr.GetDescription();
        public string Description => _cmdAttr.GetDescription();
        public CommandInfo Parent { get; }

        #endregion Fields

        #region Constructors

        private FlagInfo(PropertyInfo property, CommandInfo parent)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            if (!property.CanWrite)
                throw new ArgumentException("Cannot apply a CommandFlag to read-only properties", nameof(property));
            _cmdAttr = CommandFlagAttribute.GetFor(Property);
            Parent = parent;
        }

        #endregion Constructors
    }
}