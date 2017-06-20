using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TitanBot.TypeReaders
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

    public struct TypeReaderResponse
    {
        public IReadOnlyCollection<TypeReaderValue> Values { get; }
        
        public string Message { get; }
        public bool IsSuccess { get; }
        public object Best => Values?.OrderByDescending(v => v.Score).Select(v => v.Value).FirstOrDefault();

        private TypeReaderResponse(IReadOnlyCollection<TypeReaderValue> values, bool success, string message)
        {
            Values = values;
            Message = message;
            IsSuccess = success;
        }

        public static TypeReaderResponse FromSuccess(object value)
            => new TypeReaderResponse(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), true, null);
        public static TypeReaderResponse FromSuccess(TypeReaderValue value)
            => new TypeReaderResponse(ImmutableArray.Create(value), true, null);
        public static TypeReaderResponse FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
            => new TypeReaderResponse(values, true, null);
        public static TypeReaderResponse FromError(string message)
            => new TypeReaderResponse(null, false, message);

        public override string ToString() => IsSuccess ? "Success" : Message;
    }
}
