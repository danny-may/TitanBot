namespace TitanBot.Core.Models
{
    public struct Optional<T>
    {
        #region Statics

        public static Optional<T> Default = new Optional<T>();

        public static Optional<T> Create(T value)
            => new Optional<T>(value);

        public static Optional<T> Create()
            => new Optional<T>();

        #endregion Statics

        #region Fields

        public T Value { get; }
        public bool IsSet { get; }

        #endregion Fields

        #region Constructors

        public Optional(T value)
        {
            Value = value;
            IsSet = true;
        }

        #endregion Constructors

        #region Conversions

        public static implicit operator T(Optional<T> optional)
            => optional.Value;

        public static implicit operator Optional<T>(T value)
            => new Optional<T>(value);

        #endregion Conversions
    }

    public static class Optional
    {
        #region Statics

        public static Optional<T> Create<T>(T value)
            => Optional<T>.Create(value);

        public static Optional<T> Create<T>()
            => Optional<T>.Create();

        #endregion Statics
    }
}