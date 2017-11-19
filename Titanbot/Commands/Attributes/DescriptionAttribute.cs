using System;
using System.Reflection;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>()?.GetDescription();
        public bool ExistsOn(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Description { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public DescriptionAttribute(string description, DisplayType displayType = DisplayType.Key)
        {
            Description = description;
            DisplayType = displayType;
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetDescription()
            => DisplayType.BuildFor(Description);

        #endregion Methods
    }
}