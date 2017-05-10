using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TitanBot2.TypeReaders
{
    public struct TypeReaderValue
    {
        public object Value { get; }
        public float Score { get; }

        public TypeReaderValue(object value, float score)
        {
            Value = value;
            Score = score;
        }

        public override string ToString() => Value?.ToString();
    }

    public struct TypeReaderResult
    {
        public IReadOnlyCollection<TypeReaderValue> Values { get; }
        
        public string Message { get; }
        public bool IsSuccess { get; }
        public object Best => Values?.OrderByDescending(v => v.Score).Select(v => v.Value).FirstOrDefault();

        private TypeReaderResult(IReadOnlyCollection<TypeReaderValue> values, bool success, string message)
        {
            Values = values;
            Message = message;
            IsSuccess = success;
        }

        public static TypeReaderResult FromSuccess(object value)
            => new TypeReaderResult(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), true, null);
        public static TypeReaderResult FromSuccess(TypeReaderValue value)
            => new TypeReaderResult(ImmutableArray.Create(value), true, null);
        public static TypeReaderResult FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
            => new TypeReaderResult(values, true, null);
        public static TypeReaderResult FromError(string message)
            => new TypeReaderResult(null, false, message);

        public override string ToString() => IsSuccess ? "Success" : Message;
    }
}
