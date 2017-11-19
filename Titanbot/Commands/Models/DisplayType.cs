using Titansmasher.Services.Display;
using Titansmasher.Services.Display.Interfaces;

namespace Titanbot.Commands
{
    public enum DisplayType
    {
        /// <summary>
        /// Uses the value given as a key to look up a translation string
        /// </summary>
        Key,
        /// <summary>
        /// Uses the value as is provided
        /// </summary>
        Literal
    }

    public static class DisplayTypeMethods
    {
        public static IDisplayable<string> BuildFor(this DisplayType type, string text)
        {
            switch (type)
            {
                case DisplayType.Key:
                    return new Translation(text);
                case DisplayType.Literal:
                    return new TextLiteral(text);
                default:
                    return new TextLiteral(text);
            }
        }
    }
}