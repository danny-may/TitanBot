using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Titanbot.Command
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CommandFlagAttribute : Attribute
    {
        #region Statics

        public static CommandFlagAttribute GetFor(PropertyInfo property)
            => property.GetCustomAttribute<CommandFlagAttribute>();
        public static bool ExistsOn(PropertyInfo property)
            => property.GetCustomAttribute<CommandFlagAttribute>() != null;

        #endregion Statics

        #region Fields

        public char Id { get; }
        public string Name { get; }
        public string Description { get; }
        //public LocalisationType LocalisationType { get; }

        #endregion Fields

        #region Constructors

        public CommandFlagAttribute(char id) : this(id, null, null)
        {
        }

        public CommandFlagAttribute(char id, string name) : this(id, name, null)
        {
        }

        public CommandFlagAttribute(char id, string name, string description)//, LocalisationType localisationType = LocalisationType.Key)
        {
            Id = Regex.IsMatch($"{id}", "[a-zA-Z]") ? id : throw new ArgumentException("Field must be a letter character", nameof(id));
            Name = name;
            Description = description;
            //LocalisationType = LocalisationType;
        }

        public CommandFlagAttribute(string name, string description) ://, LocalisationType localisationType = LocalisationType.Key) :
            this(!string.IsNullOrWhiteSpace(name) ?
                    name[0] :
                    throw new ArgumentException("Field cannot be null or whitespace", nameof(name)),
                 name,
                 description)//,
                             //localisationType)
        {
        }

        public CommandFlagAttribute(string name) : this(name, null)
        {
        }

        #endregion Constructors

        #region Methods

        //public ILocalisable<string> GetDescription()
        //    => LocalisationType.BuildNew(Description);

        public string GetDescription()
            => Description;

        #endregion Methods
    }
}