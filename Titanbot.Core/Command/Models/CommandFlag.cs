using System;

namespace Titanbot.Core.Command.Models
{
    public struct CommandFlag
    {
        #region Fields

        public string Key { get; }
        public string RawValue { get; set; }

        #endregion Fields

        #region Constructors

        public CommandFlag(string key, string text)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            RawValue = text ?? throw new ArgumentNullException(nameof(text));
        }

        #endregion Constructors

        #region Methods

        public T ReadAs<T>()
            => (T)Convert.ChangeType(RawValue, typeof(T)); //TODO: Impliment using typereaders

        #endregion Methods

        #region Overrides

        public override string ToString()
            => $"{Key}: {RawValue}";

        #endregion Overrides
    }
}