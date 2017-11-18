using System;

namespace Titanbot.Commands.Models
{
    public struct FlagValue
    {
        #region Fields

        public string Key { get; }
        public string RawValue { get; set; }

        #endregion Fields

        #region Constructors

        public FlagValue(string key, string text)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrWhiteSpace(Key)) throw new ArgumentException("Argument must be non-whitespace", nameof(key));
            Key = Key.TrimStart('-');
            RawValue = text ?? throw new ArgumentNullException(nameof(text));
        }

        #endregion Constructors

        #region Methods

        public bool ReadAs<T>(out T result)
        {
            result = default(T);
            return false;
        }

        #endregion Methods

        #region Overrides

        public override string ToString()
            => $"{Key}: {RawValue}";

        #endregion Overrides
    }
}