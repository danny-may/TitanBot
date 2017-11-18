using System;
using System.Reflection;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UsageAttribute : Attribute
    {
        #region Statics

        public static string /*ILocalisable<string>*/ GetFor(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>()?.GetUsage();
        public bool ExistsOn(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>() != null;

        #endregion Statics

        #region Fields

        public string Usage { get; }
        //public LocalisationType LocalisationType { get; }

        #endregion Fields

        #region Constructors

        public UsageAttribute(string usage)//, LocalisationType localisationType = LocalisationType.Key)
        {
            Usage = usage;
            //LocalisationType = localisationType;
        }

        #endregion Constructors

        #region Methods

        //private ILocalisable<string> GetUsage()
        //    => LocalisationType.BuildNew(Usage);

        private string GetUsage()
            => Usage;

        #endregion Methods
    }
}