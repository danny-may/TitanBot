using System;
using System.Reflection;
using Titansmasher.Extensions;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class UsageAttribute : DisplayableBaseAttribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(MethodInfo info)
            => info.GetCustomAttribute<UsageAttribute>()?.GetUsage(info)
                ?? DisplayType.Key.BuildFor(DefaultKey(info));

        private static string DefaultKey(MethodInfo info)
            => GetTranslationKey(info.DeclaringType, "Usage", info.Name);

        #endregion Statics

        #region Fields

        public string Usage { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public UsageAttribute(string usage, DisplayType displayType = DisplayType.Key)
        {
            Usage = usage.NullIfWhitespace() ?? throw new ArgumentException("Argument cannot be null or whitespace", nameof(usage));
            displayType = DisplayType;
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetUsage(MethodInfo info)
            => DisplayType.BuildFor(Usage ?? DefaultKey(info));

        #endregion Methods
    }
}