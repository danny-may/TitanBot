namespace TitanBot2.TypeReaders
{
    public abstract class TypeReaderResponse
    {
        public bool IsSuccess { get; }
        protected object _value { get; }
        public string Message { get; }

        protected TypeReaderResponse(bool success, object value, string message)
        {
            IsSuccess = success;
            _value = value;
            Message = message;
        }

        public static TypeReaderResponse<T> FromSuccess<T>(T value)
            => TypeReaderResponse<T>.FromSuccess(value);

        public static TypeReaderResponse<T> FromError<T>(string message)
            => TypeReaderResponse<T>.FromError(message);
    }

    public class TypeReaderResponse<T> : TypeReaderResponse
    {
        public T Value { get { return (T)_value; } }

        private TypeReaderResponse(bool success, T value, string message) : base(success, value, message) { }

        public static TypeReaderResponse<T> FromSuccess(T value)
            => new TypeReaderResponse<T>(true, value, null);

        public static TypeReaderResponse<T> FromError(string message)
            => new TypeReaderResponse<T>(false, default(T), message);
    }
}
