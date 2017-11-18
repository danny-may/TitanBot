using System;
using System.Reflection;

namespace Titanbot.Core.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        #region Statics

        public static string /*ILocalisable<string>*/ GetFor(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>()?.GetDescription();
        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Description { get; }
        //public LocalisationType LocalisationType { get; }

        #endregion Fields

        #region Constructors

        public DescriptionAttribute(string description)//, LocalisationType localisationType = LocalisationType.Key)
        {
            Description = description;
            //LocalisationType = localisationType;
        }

        #endregion Constructors

        #region Methods

        //private ILocalisable<string> GetDescription()
        //    => LocalisationType.BuildNew(Description);

        private string GetDescription()
            => Description;

        #endregion Methods
    }
}