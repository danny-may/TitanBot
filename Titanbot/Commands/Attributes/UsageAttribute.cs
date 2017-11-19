using System;
using System.Reflection;
using Titansmasher.Services.Displaying.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UsageAttribute : Attribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>()?.GetUsage();
        public bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Usage { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public UsageAttribute(string usage, DisplayType displayType = DisplayType.Key)
        {
            Usage = usage;
            displayType = DisplayType;
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetUsage()
            => DisplayType.BuildFor(Usage);

        #endregion Methods
    }
}