using System;
using Titansmasher.Extensions;

namespace Titanbot.Commands
{
    public abstract class DisplayableBaseAttribute : Attribute
    {
        protected static string GetTranslationKey(Type commandType, string area, string path = null)
            => $"Commands.{commandType.Name}.{area ?? throw new ArgumentNullException(nameof(area))}" +
            (path.NullIfWhitespace() == null ? "" : "." + path);
    }
}