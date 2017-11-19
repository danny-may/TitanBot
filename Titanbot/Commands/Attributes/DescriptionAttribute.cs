using System;
using System.Reflection;
using Titansmasher.Extensions;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : DisplayableBaseAttribute
    {
        #region Statics

        public static IDisplayable<string> GetFor(Type info)
            => info.GetCustomAttribute<DescriptionAttribute>()?.GetDescription(info)
                ?? DisplayType.Key.BuildFor(DefaultKey(info));

        private static string DefaultKey(Type info)
            => GetTranslationKey(info, "description");

        #endregion Statics

        #region Fields

        public string Description { get; }
        public DisplayType DisplayType { get; }

        #endregion Fields

        #region Constructors

        public DescriptionAttribute(string description, DisplayType displayType = DisplayType.Key)
        {
            Description = description.NullIfWhitespace() ?? throw new ArgumentException("Argument cannot be null or whitespace", nameof(description));
            DisplayType = displayType;
        }

        #endregion Constructors

        #region Methods

        private IDisplayable<string> GetDescription(Type info)
            => DisplayType.BuildFor(Description ?? DefaultKey(info));

        #endregion Methods
    }
}